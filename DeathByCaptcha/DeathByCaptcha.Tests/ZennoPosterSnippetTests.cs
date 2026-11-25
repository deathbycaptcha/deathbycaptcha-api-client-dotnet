using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DeathByCaptcha.ZennoPoster;
using Xunit;

namespace DeathByCaptcha.Tests
{
    [Trait("Category", "Integration")]
    [Trait("Category", "ZennoPoster")]
    public class ZennoPosterSnippetTests
    {
        private const string DemoUrl = "https://www.google.com/recaptcha/api2/demo";

        private static bool LoadEnvFile()
        {
            try { DotNetEnv.Env.TraversePath().Load(); }
            catch { }
            return true;
        }

        private static readonly bool _envLoaded = LoadEnvFile();

        [SkippableFact]
        public async Task ZennoPoster_RecaptchaV2_Snippet_SolvesLive()
        {
            string username = Environment.GetEnvironmentVariable("DBC_USERNAME") ?? string.Empty;
            string password = Environment.GetEnvironmentVariable("DBC_PASSWORD") ?? string.Empty;

            Skip.If(
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password),
                "ZennoPoster snippet test requires DBC_USERNAME and DBC_PASSWORD.");

            string html;
            using (var http = new System.Net.Http.HttpClient())
            {
                html = await http.GetStringAsync(DemoUrl);
            }

            Match match = Regex.Match(html, "data-sitekey=\"([^\"]+)\"");
            Assert.True(match.Success, "The demo page did not expose a sitekey");

            string token = Snippets.SolveRecaptchaV2(match.Groups[1].Value, DemoUrl);
            Assert.False(string.IsNullOrWhiteSpace(token), "The snippet returned no token");
        }
    }
}
