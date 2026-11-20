// PuppeteerSharp + reCAPTCHA v2 sample for DeathByCaptcha

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DeathByCaptcha;
using PuppeteerSharp;

namespace DeathByCaptcha
{
    public class PuppeteerRecaptchaV2Example
    {
        private const string DemoUrl = "https://www.google.com/recaptcha/api2/demo";

        private static bool ParseBool(string? value, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            switch (value.Trim().ToLowerInvariant())
            {
                case "1":
                case "true":
                case "yes":
                case "on":
                    return true;
                case "0":
                case "false":
                case "no":
                case "off":
                    return false;
                default:
                    return defaultValue;
            }
        }

        private static async Task<string> ResolveChromePathAsync()
        {
            string? chromeBin = Environment.GetEnvironmentVariable("CHROME_BIN");
            if (!string.IsNullOrWhiteSpace(chromeBin) && File.Exists(chromeBin))
            {
                return chromeBin;
            }

            Console.WriteLine("Chrome for PuppeteerSharp is not installed; downloading it...");
            var installed = await new BrowserFetcher().DownloadAsync();
            return installed.GetExecutablePath();
        }

        static public async Task Main(string[] argv)
        {
            string username = Environment.GetEnvironmentVariable("DBC_USERNAME") ?? "";
            string password = Environment.GetEnvironmentVariable("DBC_PASSWORD") ?? "";
            string pageUrl = DemoUrl;
            bool headless = ParseBool(
                Environment.GetEnvironmentVariable("DBC_PUPPETEER_HEADLESS"), true);

            foreach (string arg in argv)
            {
                if (arg.Equals("--headless", StringComparison.OrdinalIgnoreCase))
                {
                    headless = true;
                    continue;
                }

                if (arg.Equals("--headed", StringComparison.OrdinalIgnoreCase))
                {
                    headless = false;
                    continue;
                }

                pageUrl = arg;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Set DBC_USERNAME and DBC_PASSWORD first.");
                return;
            }

            string chromePath = await ResolveChromePathAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = headless,
                ExecutablePath = chromePath,
                Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" },
            });

            await using var page = await browser.NewPageAsync();

            Console.WriteLine($"Navigating to {pageUrl}...");
            await page.GoToAsync(pageUrl, new NavigationOptions { Timeout = 60000 });
            await page.WaitForSelectorAsync(
                "#recaptcha-demo", new WaitForSelectorOptions { Timeout = 20000 });

            string? googleKey = await page.EvaluateFunctionAsync<string>(
                "() => document.querySelector('#recaptcha-demo').getAttribute('data-sitekey')");

            if (string.IsNullOrWhiteSpace(googleKey))
            {
                Console.WriteLine("Could not extract the reCAPTCHA sitekey from the page");
                return;
            }

            Console.WriteLine($"Sitekey: {googleKey}");

            string tokenParams = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["googlekey"] = googleKey,
                ["pageurl"] = pageUrl,
            });

            var captchaData = new Hashtable
            {
                ["type"] = 4,
                ["token_params"] = tokenParams,
            };

            Client client = new HttpClient(username, password);
            Captcha? solution = client.Decode(Client.DefaultTokenTimeout, captchaData);

            if (solution == null || string.IsNullOrWhiteSpace(solution.Text))
            {
                Console.WriteLine("No captcha solution was returned");
                return;
            }

            Console.WriteLine($"Token received ({solution.Text.Length} characters)");

            await page.EvaluateFunctionAsync<bool>(
                @"(token) => {
                    const textarea = document.getElementsByName('g-recaptcha-response')[0]
                        || document.getElementById('g-recaptcha-response');
                    if (!textarea) return false;

                    textarea.style.display = 'block';
                    textarea.value = token;
                    textarea.innerHTML = token;
                    textarea.dispatchEvent(new Event('input', { bubbles: true }));
                    textarea.dispatchEvent(new Event('change', { bubbles: true }));

                    const clients = window.___grecaptcha_cfg && window.___grecaptcha_cfg.clients;
                    if (clients) {
                        for (const key of Object.keys(clients)) {
                            const client = clients[key];
                            for (const inner of Object.keys(client)) {
                                const item = client[inner];
                                if (item && typeof item.callback === 'function') {
                                    item.callback(token);
                                    return true;
                                }
                                if (item && item.W && typeof item.W.callback === 'function') {
                                    item.W.callback(token);
                                    return true;
                                }
                            }
                        }
                    }
                    return true;
                }",
                solution.Text);

            await page.ClickAsync("#recaptcha-demo-submit");
            await page.WaitForSelectorAsync(
                ".recaptcha-success", new WaitForSelectorOptions { Timeout = 15000 });

            string successText = await page.EvaluateFunctionAsync<string>(
                "() => document.querySelector('.recaptcha-success').textContent") ?? "Success";
            Console.WriteLine(successText.Trim());
        }
    }
}
