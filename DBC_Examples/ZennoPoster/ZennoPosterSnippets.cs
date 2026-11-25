using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using DeathByCaptcha;

namespace DeathByCaptcha.ZennoPoster
{
    /// <summary>
    /// C# snippets for ZennoPoster "Own code" cubes.
    ///
    /// Each method solves one challenge through the DeathByCaptcha API and
    /// returns the token or the text. Credentials come from the environment
    /// (DBC_USERNAME/DBC_PASSWORD or DBC_AUTHTOKEN), so nothing is stored in
    /// the project or pasted into a cube.
    ///
    /// The snippets use only the DeathByCaptcha client and the standard
    /// library, so they compile anywhere the client compiles. ZennoPoster
    /// wiring (reading the screenshot, filling the field) stays in the cube.
    /// </summary>
    public static class Snippets
    {
        private static Client CreateClient()
        {
            string authtoken = Environment.GetEnvironmentVariable("DBC_AUTHTOKEN");
            string username = Environment.GetEnvironmentVariable("DBC_USERNAME");
            string password = Environment.GetEnvironmentVariable("DBC_PASSWORD");

            if (!string.IsNullOrWhiteSpace(authtoken))
            {
                // The client takes the authtoken as the password with the
                // magic username "authtoken".
                return new HttpClient("authtoken", authtoken);
            }
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                return new HttpClient(username, password);
            }

            throw new InvalidOperationException(
                "Set DBC_AUTHTOKEN or DBC_USERNAME/DBC_PASSWORD before solving a CAPTCHA.");
        }

        private static string Decode(Hashtable data, int timeout)
        {
            Client client = CreateClient();
            Captcha captcha = client.Decode(timeout, data);

            if (captcha == null || string.IsNullOrWhiteSpace(captcha.Text))
            {
                throw new InvalidOperationException("DeathByCaptcha returned no solution.");
            }

            return captcha.Text;
        }

        /// <summary>reCAPTCHA v2: type 4, needs the sitekey and the page URL.</summary>
        public static string SolveRecaptchaV2(string sitekey, string pageurl, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(sitekey) || string.IsNullOrWhiteSpace(pageurl))
            {
                throw new ArgumentException("sitekey and pageurl are required.");
            }

            string tokenParams = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["googlekey"] = sitekey,
                ["pageurl"] = pageurl,
            });

            var data = new Hashtable
            {
                ["type"] = 4,
                ["token_params"] = tokenParams,
            };

            return Decode(data, timeout);
        }

        /// <summary>reCAPTCHA v3: type 5, with an optional action and minimum score.</summary>
        public static string SolveRecaptchaV3(
            string sitekey, string pageurl, string action = null, double minScore = 0, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(sitekey) || string.IsNullOrWhiteSpace(pageurl))
            {
                throw new ArgumentException("sitekey and pageurl are required.");
            }

            var fields = new Dictionary<string, string>
            {
                ["googlekey"] = sitekey,
                ["pageurl"] = pageurl,
            };
            if (!string.IsNullOrWhiteSpace(action))
            {
                fields["action"] = action;
            }
            if (minScore > 0)
            {
                fields["min_score"] = minScore.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            var data = new Hashtable
            {
                ["type"] = 5,
                ["token_params"] = JsonSerializer.Serialize(fields),
            };

            return Decode(data, timeout);
        }

        /// <summary>Cloudflare Turnstile: type 12. The token is valid from this exit IP.</summary>
        public static string SolveTurnstile(string sitekey, string pageurl, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(sitekey) || string.IsNullOrWhiteSpace(pageurl))
            {
                throw new ArgumentException("sitekey and pageurl are required.");
            }

            string turnstileParams = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["sitekey"] = sitekey,
                ["pageurl"] = pageurl,
            });

            var data = new Hashtable
            {
                ["type"] = 12,
                ["turnstile_params"] = turnstileParams,
            };

            return Decode(data, timeout);
        }

        /// <summary>Amazon WAF: type 16, with the sitekey, iv and context from the page.</summary>
        public static string SolveAmazonWaf(
            string sitekey, string pageurl, string iv, string context, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(sitekey) || string.IsNullOrWhiteSpace(pageurl) ||
                string.IsNullOrWhiteSpace(iv) || string.IsNullOrWhiteSpace(context))
            {
                throw new ArgumentException("sitekey, pageurl, iv and context are required.");
            }

            string wafParams = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["sitekey"] = sitekey,
                ["pageurl"] = pageurl,
                ["iv"] = iv,
                ["context"] = context,
            });

            var data = new Hashtable
            {
                ["type"] = 16,
                ["waf_params"] = wafParams,
            };

            return Decode(data, timeout);
        }

        /// <summary>Image CAPTCHA: type 0, from a file on disk (a screenshot from the cube).</summary>
        public static string SolveImage(string imagePath, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                throw new ArgumentException("A readable image path is required.", nameof(imagePath));
            }

            Client client = CreateClient();
            Captcha captcha = client.Decode(imagePath, timeout);

            if (captcha == null || string.IsNullOrWhiteSpace(captcha.Text))
            {
                throw new InvalidOperationException("DeathByCaptcha returned no solution.");
            }

            return captcha.Text;
        }

        /// <summary>Account balance in US cents, useful as a cube precondition.</summary>
        public static double GetBalance()
        {
            Client client = CreateClient();
            return client.GetBalance();
        }
    }
}
