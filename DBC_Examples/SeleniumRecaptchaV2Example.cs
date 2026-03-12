// Selenium + reCAPTCHA v2 token sample

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

namespace DeathByCaptcha
{
    public class SeleniumRecaptchaV2Example
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

        static public void Main(string[] argv)
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
                Environment.GetEnvironmentVariable("DBC_SELENIUM_HEADLESS"),
                true);

            var options = new FirefoxOptions();
            if (headless)
            {
                options.AddArgument("--headless");
            }

            using IWebDriver driver = new FirefoxDriver(options);
            driver.Navigate().GoToUrl(pageUrl);

            IWebElement? captchaElement = new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(
                ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("recaptcha-demo"))).FirstOrDefault();

            if (captchaElement == null)
            {
                Console.WriteLine("Could not find recaptcha container element with id 'recaptcha-demo'.");
                return;
            }

            string googleKey = captchaElement.GetAttribute("data-sitekey") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(googleKey))
            {
                Console.WriteLine("Could not extract data-sitekey from page.");
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

            // Use HTTP client for token workflows.
            Client client = new HttpClient(username, password);
            Captcha? solution = client.Decode(Client.DefaultTokenTimeout, captchaData);

            if (solution == null || string.IsNullOrWhiteSpace(solution.Text))
            {
                Console.WriteLine("No captcha solution was returned.");
                return;
            }

            ((IJavaScriptExecutor)driver).ExecuteScript(
                "document.getElementById('g-recaptcha-response').value = arguments[0];",
                solution.Text);

            IWebElement submitButton = driver.FindElement(By.Id("recaptcha-demo-submit"));
            submitButton.Click();

            try
            {
                IWebElement successElement = new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(
                    ExpectedConditions.ElementIsVisible(By.ClassName("recaptcha-success")));
                Console.WriteLine(successElement.Text);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Success message was not found after submitting token.");
            }
        }
    }
}
