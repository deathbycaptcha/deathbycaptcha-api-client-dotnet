using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DeathByCaptcha;
using PuppeteerSharp;
using Xunit;

namespace DeathByCaptcha.Tests
{
    [Trait("Category", "Integration")]
    [Trait("Category", "Puppeteer")]
    public class PuppeteerIntegrationTests
    {
        private static bool LoadEnvFile()
        {
            try { DotNetEnv.Env.TraversePath().Load(); }
            catch { }
            return true;
        }

        private static readonly bool _envLoaded = LoadEnvFile();

        private static bool ParseBool(string value, bool defaultValue)
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
            string chromeBin = Environment.GetEnvironmentVariable("CHROME_BIN");
            if (!string.IsNullOrWhiteSpace(chromeBin) && File.Exists(chromeBin))
            {
                return chromeBin;
            }

            var installed = await new BrowserFetcher().DownloadAsync();
            return installed.GetExecutablePath();
        }

        [SkippableFact]
        public async Task Puppeteer_RecaptchaV2_Demo_Headless_ResolvesToken()
        {
            Console.WriteLine("[PUPPETEER-IT] START");
            string username = Environment.GetEnvironmentVariable("DBC_USERNAME") ?? string.Empty;
            string password = Environment.GetEnvironmentVariable("DBC_PASSWORD") ?? string.Empty;

            Skip.If(
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password),
                "Puppeteer integration test requires DBC_USERNAME and DBC_PASSWORD.");

            string pageUrl = Environment.GetEnvironmentVariable("DBC_PUPPETEER_PAGE_URL")
                             ?? "https://www.google.com/recaptcha/api2/demo";
            bool headless = ParseBool(
                Environment.GetEnvironmentVariable("DBC_PUPPETEER_HEADLESS"), true);
            Console.WriteLine($"[PUPPETEER-IT] pageUrl={pageUrl}");
            Console.WriteLine($"[PUPPETEER-IT] headless={headless}");

            string chromePath = await ResolveChromePathAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = headless,
                ExecutablePath = chromePath,
                Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" },
            });

            await using var page = await browser.NewPageAsync();

            await page.GoToAsync(pageUrl, new NavigationOptions { Timeout = 60000 });
            await page.WaitForSelectorAsync(
                "#recaptcha-demo", new WaitForSelectorOptions { Timeout = 20000 });

            string googleKey = await page.EvaluateFunctionAsync<string>(
                "() => document.querySelector('#recaptcha-demo').getAttribute('data-sitekey')");
            Assert.False(string.IsNullOrWhiteSpace(googleKey), "Could not extract the sitekey");

            string tokenParams = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["googlekey"] = googleKey!,
                ["pageurl"] = pageUrl,
            });

            var client = new HttpClient(username, password);
            var captchaData = new Hashtable
            {
                ["type"] = 4,
                ["token_params"] = tokenParams,
            };

            Captcha solution = client.Decode(Client.DefaultTokenTimeout, captchaData);
            Assert.NotNull(solution);
            Assert.False(string.IsNullOrWhiteSpace(solution.Text));
            Assert.True(solution.Text.Length > 0, "The token should not be empty");

            bool injected = await page.EvaluateFunctionAsync<bool>(
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
            Assert.True(injected, "The token could not be injected");

            await page.ClickAsync("#recaptcha-demo-submit");
            await page.WaitForSelectorAsync(
                ".recaptcha-success", new WaitForSelectorOptions { Timeout = 15000 });

            string successText = await page.EvaluateFunctionAsync<string>(
                "() => document.querySelector('.recaptcha-success').textContent") ?? string.Empty;
            Assert.Contains("Verification Success", successText);
            Console.WriteLine("[PUPPETEER-IT] success");
        }
    }
}
