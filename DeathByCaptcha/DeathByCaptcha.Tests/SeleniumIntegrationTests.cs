using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DeathByCaptcha;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Xunit;

namespace DeathByCaptcha.Tests
{
    [Trait("Category", "Integration")]
    [Trait("Category", "Selenium")]
    public class SeleniumIntegrationTests
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
        public void Selenium_RecaptchaV2_Demo_Headless_ResolvesToken()
        {
            Console.WriteLine("[SELENIUM-IT] START");
            string username = Environment.GetEnvironmentVariable("DBC_USERNAME") ?? string.Empty;
            string password = Environment.GetEnvironmentVariable("DBC_PASSWORD") ?? string.Empty;

            Skip.If(
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password),
                "Selenium integration test requires DBC_USERNAME and DBC_PASSWORD.");

            string pageUrl = Environment.GetEnvironmentVariable("DBC_SELENIUM_PAGE_URL")
                             ?? "https://www.google.com/recaptcha/api2/demo";

            bool headless = ParseBool(Environment.GetEnvironmentVariable("DBC_SELENIUM_HEADLESS"), true);
            Console.WriteLine($"[SELENIUM-IT] pageUrl={pageUrl}");
            Console.WriteLine($"[SELENIUM-IT] headless={headless}");

            var options = new FirefoxOptions();
            if (headless)
            {
                options.AddArgument("--headless");
            }

            using IWebDriver driver = new FirefoxDriver(options);
            driver.Navigate().GoToUrl(pageUrl);

            IWebElement? captchaElement = new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(
                ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("recaptcha-demo"))).FirstOrDefault();

            Assert.NotNull(captchaElement);
            Console.WriteLine("[SELENIUM-IT] recaptcha container found");

            string googleKey = captchaElement.GetAttribute("data-sitekey") ?? string.Empty;
            Assert.False(string.IsNullOrWhiteSpace(googleKey));
            Console.WriteLine($"[SELENIUM-IT] sitekey-length={googleKey.Length}");

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
                Console.WriteLine("[SELENIUM-IT] requesting token from DBC");
                Captcha solution = client.Decode(Client.DefaultTokenTimeout, captchaData);
                Assert.NotNull(solution);
                Assert.False(string.IsNullOrWhiteSpace(solution.Text));
                Console.WriteLine($"[SELENIUM-IT] token received captchaId={solution.Id} token-length={solution.Text.Length}");

                ((IJavaScriptExecutor)driver).ExecuteScript(
                    "document.getElementById('g-recaptcha-response').value = arguments[0];",
                    solution.Text);
                Console.WriteLine("[SELENIUM-IT] token injected into g-recaptcha-response");

                IWebElement submitButton = driver.FindElement(By.Id("recaptcha-demo-submit"));
                submitButton.Click();
                Console.WriteLine("[SELENIUM-IT] submit clicked");

                IWebElement successElement = new WebDriverWait(driver, TimeSpan.FromSeconds(15)).Until(
                    ExpectedConditions.ElementIsVisible(By.ClassName("recaptcha-success")));

                string successText = successElement.Text ?? string.Empty;
                Assert.Contains("Verification Success", successText, StringComparison.OrdinalIgnoreCase);
                Console.WriteLine($"[SELENIUM-IT] successText='{successText}'");
                Console.WriteLine("[SELENIUM-IT] PASS");
            }
            finally
            {
                client.Close();
                Console.WriteLine("[SELENIUM-IT] END");
            }
        }
    }
}
