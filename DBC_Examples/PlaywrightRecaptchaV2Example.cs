// Playwright + reCAPTCHA v2 token sample

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DeathByCaptcha;
using Microsoft.Playwright;

namespace DeathByCaptcha
{
    public class PlaywrightRecaptchaV2Example
    {
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

        static public async Task Main(string[] argv)
        {
            string username = Environment.GetEnvironmentVariable("DBC_USERNAME") ?? "DBC_USERNAME";
            string password = Environment.GetEnvironmentVariable("DBC_PASSWORD") ?? "DBC_PASSWORD";
            string pageUrl = "https://www.google.com/recaptcha/api2/demo";

            bool? headlessArg = null;
            foreach (string arg in argv)
            {
                if (arg.Equals("--headless", StringComparison.OrdinalIgnoreCase))
                {
                    headlessArg = true;
                    continue;
                }

                if (arg.Equals("--headed", StringComparison.OrdinalIgnoreCase))
                {
                    headlessArg = false;
                    continue;
                }

                pageUrl = arg;
            }

            bool headless = headlessArg ?? ParseBool(
                Environment.GetEnvironmentVariable("DBC_PLAYWRIGHT_HEADLESS"),
                true);

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless
            });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            try
            {
                Console.WriteLine($"Navigating to {pageUrl}...");
                await page.GotoAsync(pageUrl, new PageGotoOptions { Timeout = 60000 });

                Console.WriteLine("Looking for recaptcha-demo element...");
                await page.WaitForSelectorAsync("#recaptcha-demo", new PageWaitForSelectorOptions { Timeout = 20000 });
                Console.WriteLine("Element found");

                string? googleKey = await page.GetAttributeAsync("#recaptcha-demo", "data-sitekey");

                if (string.IsNullOrWhiteSpace(googleKey))
                {
                    Console.WriteLine("Trying alternative method to get sitekey...");
                    googleKey = await page.EvaluateAsync<string>(
                        "document.getElementById('recaptcha-demo').getAttribute('data-sitekey')");
                }

                Console.WriteLine($"GoogleKey: {googleKey}");

                if (string.IsNullOrWhiteSpace(googleKey))
                {
                    Console.WriteLine("ERROR: Could not extract sitekey from page");
                    return;
                }

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

                Client client = new HttpClient(username, password);
                Captcha? solution = client.Decode(Client.DefaultTokenTimeout, captchaData);

                if (solution == null || string.IsNullOrWhiteSpace(solution.Text))
                {
                    Console.WriteLine("No captcha solution was returned.");
                    return;
                }

                Console.WriteLine($"Solution: {solution.Text}");

                await page.EvaluateAsync(
                    "value => document.getElementById('g-recaptcha-response').value = value;",
                    solution.Text);

                await page.ClickAsync("#recaptcha-demo-submit");

                try
                {
                    await page.WaitForSelectorAsync(".recaptcha-success", new PageWaitForSelectorOptions { Timeout = 10000 });
                    string successText = await page.TextContentAsync(".recaptcha-success") ?? "Success";
                    Console.WriteLine(successText);
                }
                catch (PlaywrightException)
                {
                    Console.WriteLine("Success message was not found after submitting token.");
                }
            }
            finally
            {
                await browser.CloseAsync();
            }
        }
    }
}
