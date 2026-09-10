using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DeathByCaptcha;
using Microsoft.Playwright;
using Xunit;

namespace DeathByCaptcha.Tests
{
    [Trait("Category", "Integration")]
    [Trait("Category", "Playwright")]
    public class PlaywrightIntegrationTests
    {
        private static readonly bool _envLoaded = LoadEnvFile();

        private static bool LoadEnvFile()
        {
            try { DotNetEnv.Env.TraversePath().Load(); }
            catch { }
            return true;
        }

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

        [SkippableFact]
        public async Task Playwright_RecaptchaV2_Demo_Headless_ResolvesToken()
        {
            Console.WriteLine("[PLAYWRIGHT-IT] START");
            string username = Environment.GetEnvironmentVariable("DBC_USERNAME") ?? string.Empty;
            string password = Environment.GetEnvironmentVariable("DBC_PASSWORD") ?? string.Empty;

            Skip.If(
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password),
                "Playwright integration test requires DBC_USERNAME and DBC_PASSWORD.");

            string pageUrl = Environment.GetEnvironmentVariable("DBC_PLAYWRIGHT_PAGE_URL")
                             ?? "https://www.google.com/recaptcha/api2/demo";

            bool headless = ParseBool(Environment.GetEnvironmentVariable("DBC_PLAYWRIGHT_HEADLESS"), true);
            Console.WriteLine($"[PLAYWRIGHT-IT] pageUrl={pageUrl}");
            Console.WriteLine($"[PLAYWRIGHT-IT] headless={headless}");

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless
            });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            try
            {
                Console.WriteLine("[PLAYWRIGHT-IT] navigating to page...");
                await page.GotoAsync(pageUrl, new PageGotoOptions { Timeout = 60000 });

                Console.WriteLine("[PLAYWRIGHT-IT] looking for recaptcha-demo element...");
                await page.WaitForSelectorAsync("#recaptcha-demo", new PageWaitForSelectorOptions { Timeout = 20000 });
                Console.WriteLine("[PLAYWRIGHT-IT] recaptcha container found");

                string? googleKey = await page.GetAttributeAsync("#recaptcha-demo", "data-sitekey");

                if (string.IsNullOrWhiteSpace(googleKey))
                {
                    googleKey = await page.EvaluateAsync<string>(
                        "document.getElementById('recaptcha-demo').getAttribute('data-sitekey')");
                }

                Assert.False(string.IsNullOrWhiteSpace(googleKey));
                Console.WriteLine($"[PLAYWRIGHT-IT] sitekey-length={googleKey!.Length}");

                string tokenParams = JsonSerializer.Serialize(new Dictionary<string, string>
                {
                    ["googlekey"] = googleKey,
                    ["pageurl"] = pageUrl
                });

                var captchaData = new Hashtable
                {
                    ["type"] = 4,
                    ["token_params"] = tokenParams
                };

                Client client = new HttpClient(username, password) { Verbose = false };
                try
                {
                    Console.WriteLine("[PLAYWRIGHT-IT] requesting token from DBC");
                    Captcha solution;
                    try
                    {
                        solution = client.Decode(Client.DefaultTokenTimeout, captchaData);
                    }
                    catch (AccessDeniedException)
                    {
                        Skip.If(true, "DBC API access denied — check credentials and/or balance.");
                        return;
                    }
                    Assert.NotNull(solution);
                    Assert.False(string.IsNullOrWhiteSpace(solution.Text));
                    Console.WriteLine($"[PLAYWRIGHT-IT] token received captchaId={solution.Id} token-length={solution.Text.Length}");

                    await page.EvaluateAsync(
                        "value => document.getElementById('g-recaptcha-response').value = value;",
                        solution.Text);
                    Console.WriteLine("[PLAYWRIGHT-IT] token injected into g-recaptcha-response");

                    await page.ClickAsync("#recaptcha-demo-submit");
                    Console.WriteLine("[PLAYWRIGHT-IT] submit clicked");

                    await page.WaitForSelectorAsync(".recaptcha-success", new PageWaitForSelectorOptions { Timeout = 15000 });
                    string successText = await page.TextContentAsync(".recaptcha-success") ?? string.Empty;
                    Assert.Contains("Verification Success", successText, StringComparison.OrdinalIgnoreCase);
                    Console.WriteLine($"[PLAYWRIGHT-IT] successText='{successText}'");
                    Console.WriteLine("[PLAYWRIGHT-IT] PASS");
                }
                finally
                {
                    client.Close();
                    Console.WriteLine("[PLAYWRIGHT-IT] END");
                }
            }
            finally
            {
                await browser.CloseAsync();
            }
        }
    }
}
