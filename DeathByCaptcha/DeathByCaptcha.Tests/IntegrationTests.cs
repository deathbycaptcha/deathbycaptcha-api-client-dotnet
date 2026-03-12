// Integration tests against the real DBC API.
// These tests are skipped automatically when credentials are not set.
//
// To run:
//   export DBC_USERNAME=your_username
//   export DBC_PASSWORD=your_password
//   dotnet test --filter "Category=Integration"
//
// To run full API-surface workflows (slower and more costly):
//   export DBC_INTEGRATION_FULL=true

using System;
using System.IO;
using System.Threading;
using Xunit;
using DeathByCaptcha;

namespace DeathByCaptcha.Tests
{
    [Trait("Category", "Integration")]
    public class IntegrationTests
    {
        /// <summary>
        /// Auto-load a .env file from the repo root (or any parent) on first test run.
        /// This is a local-dev convenience; in CI the variables come from the runner environment.
        /// </summary>
        private static readonly bool _envLoaded = LoadEnvFile();

        private static bool LoadEnvFile()
        {
            // TraversePath walks from CWD upwards until it finds a .env file.
            // Silently does nothing when the file is absent.
            try { DotNetEnv.Env.TraversePath().Load(); }
            catch { /* env file missing or unreadable – CI will supply vars directly */ }
            return true;
        }
        /// <summary>Solving timeout in seconds for Decode calls.</summary>
        private const int DecodeTimeoutSeconds = 120;

        /// <summary>
        /// Reads DBC_USERNAME / DBC_PASSWORD from environment.
        /// Skips the calling test if either is absent.
        /// </summary>
        private static void GetCredentials(out string username, out string password)
        {
            username = Environment.GetEnvironmentVariable("DBC_USERNAME") ?? string.Empty;
            password = Environment.GetEnvironmentVariable("DBC_PASSWORD") ?? string.Empty;

            Skip.If(
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password),
                "Integration tests require DBC_USERNAME and DBC_PASSWORD " +
                "environment variables. Set them and re-run with " +
                "--filter \"Category=Integration\".");
        }

            private static void RequireFullIntegrationEnabled()
            {
                string full = Environment.GetEnvironmentVariable("DBC_INTEGRATION_FULL") ?? string.Empty;
                bool enabled = string.Equals(full, "1", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(full, "true", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(full, "yes", StringComparison.OrdinalIgnoreCase);

                Skip.If(!enabled,
                "Full API-surface integration tests are disabled by default. " +
                "Set DBC_INTEGRATION_FULL=true to run them.");
            }

        /// <summary>Absolute path to the normal.jpg test image copied to the test output directory.</summary>
        private static string NormalCaptchaImagePath =>
            Path.Combine(AppContext.BaseDirectory, "images", "normal.jpg");

        private static void AssertUserAndBalance(Client client, string label)
        {
            User userByMethod = client.GetUser();
            User userByProperty = client.User;
            double balanceByMethod = client.GetBalance();
            double balanceByProperty = client.Balance;

            Assert.NotNull(userByMethod);
            Assert.NotNull(userByProperty);
            Assert.True(userByMethod.LoggedIn, $"[{label}] User.LoggedIn should be true");
            Assert.True(userByMethod.Id > 0, $"[{label}] User.Id should be > 0");
            Assert.True(userByMethod.Balance >= 0d, $"[{label}] User.Balance should be >= 0");
            Assert.True(balanceByMethod >= 0d, $"[{label}] GetBalance() should be >= 0");
            Assert.True(balanceByProperty >= 0d, $"[{label}] Balance property should be >= 0");

            Console.WriteLine($"[{label}] User id={userByMethod.Id} balance={userByMethod.Balance:F2} cents");
        }

        private static Captcha WaitSolvedCaptcha(Client client, Captcha uploaded, string label)
        {
            Assert.NotNull(uploaded);
            Assert.True(uploaded.Uploaded, $"[{label}] Uploaded captcha should have Uploaded=true");
            Assert.True(uploaded.Id > 0, $"[{label}] Uploaded captcha id should be > 0");

            var deadline = DateTime.UtcNow.AddSeconds(DecodeTimeoutSeconds);
            Captcha current = uploaded;

            while (DateTime.UtcNow < deadline && !current.Solved)
            {
                Thread.Sleep(2000);
                current = client.GetCaptcha(current.Id);
                Assert.NotNull(current);
            }

            Assert.True(current.Solved, $"[{label}] Upload/GetCaptcha polling should reach solved state");
            Assert.True(current.Correct, $"[{label}] Solved captcha should be Correct");
            Assert.False(string.IsNullOrWhiteSpace(current.Text), $"[{label}] Solved captcha text should not be empty");

            return current;
        }

        private static void RunFullApiSurface(Client client, string label)
        {
            string imgPath = NormalCaptchaImagePath;
            Assert.True(File.Exists(imgPath), $"[{label}] Test image not found: {imgPath}");
            byte[] imgBytes = File.ReadAllBytes(imgPath);

            // 1) Account and server-side user status endpoints.
            AssertUserAndBalance(client, label);

            // 2) Upload + GetCaptcha/GetText + Report endpoints.
            Captcha uploaded = client.Upload(imgBytes);
            Captcha solvedViaUpload = WaitSolvedCaptcha(client, uploaded, label);

            Captcha byId = client.GetCaptcha(solvedViaUpload.Id);
            Captcha byObj = client.GetCaptcha(solvedViaUpload);
            string textById = client.GetText(solvedViaUpload.Id);
            string textByObj = client.GetText(solvedViaUpload);

            Assert.NotNull(byId);
            Assert.NotNull(byObj);
            Assert.Equal(solvedViaUpload.Id, byId.Id);
            Assert.Equal(solvedViaUpload.Id, byObj.Id);
            Assert.False(string.IsNullOrWhiteSpace(textById));
            Assert.False(string.IsNullOrWhiteSpace(textByObj));
            Assert.Equal(textById, textByObj);

            bool reportById = client.Report(solvedViaUpload.Id);
            bool reportByObj = client.Report(solvedViaUpload);
            Console.WriteLine($"[{label}] Report(id)={reportById}, Report(captcha)={reportByObj}");

            // 3) Decode endpoint (sync overload).
            Captcha decodedSync = client.Decode(imgPath, DecodeTimeoutSeconds);
            Assert.NotNull(decodedSync);
            Assert.True(decodedSync.Solved, $"[{label}] Decode(sync) should return solved captcha");
            Assert.True(decodedSync.Correct, $"[{label}] Decode(sync) should return correct captcha");
            Assert.False(string.IsNullOrWhiteSpace(decodedSync.Text), $"[{label}] Decode(sync) text should not be empty");
            Console.WriteLine($"[{label}] Decode(sync) id={decodedSync.Id} text='{decodedSync.Text}'");

            // 4) Decode endpoint (async callback overload).
            using (var done = new ManualResetEventSlim(false))
            {
                Captcha decodedAsync = null;
                client.Decode(c =>
                {
                    decodedAsync = c;
                    done.Set();
                }, imgBytes, DecodeTimeoutSeconds);

                Assert.True(done.Wait(TimeSpan.FromSeconds(DecodeTimeoutSeconds + 30)),
                    $"[{label}] Decode(callback) timed out waiting for callback");
                Assert.NotNull(decodedAsync);
                Assert.True(decodedAsync.Solved, $"[{label}] Decode(callback) should return solved captcha");
                Assert.True(decodedAsync.Correct, $"[{label}] Decode(callback) should return correct captcha");
                Assert.False(string.IsNullOrWhiteSpace(decodedAsync.Text), $"[{label}] Decode(callback) text should not be empty");
                Console.WriteLine($"[{label}] Decode(callback) id={decodedAsync.Id} text='{decodedAsync.Text}'");
            }
        }

        // ─── HTTP client ────────────────────────────────────────────────────

        [SkippableFact]
        public void Http_GetBalance_IsNonNegative()
        {
            GetCredentials(out var u, out var p);
            var client = new HttpClient(u, p) { Verbose = false };
            try
            {
                double balance = client.GetBalance();
                Assert.True(balance >= 0d,
                    $"Expected balance >= 0, got {balance}");
            }
            finally { client.Close(); }
        }

        [SkippableFact]
        public void Http_GetUser_IsLoggedInWithPositiveId()
        {
            GetCredentials(out var u, out var p);
            var client = new HttpClient(u, p) { Verbose = false };
            try
            {
                User user = client.GetUser();

                Assert.NotNull(user);
                Assert.True(user.LoggedIn,
                    "User.LoggedIn should be true for valid credentials");
                Assert.True(user.Id > 0,
                    $"User.Id should be > 0, got {user.Id}");
                Assert.True(user.Balance >= 0d,
                    $"User.Balance should be >= 0, got {user.Balance}");

                Console.WriteLine(
                    $"[HTTP] User id={user.Id} balance={user.Balance:F2} cents");
            }
            finally { client.Close(); }
        }

        [SkippableFact]
        public void Http_DecodeNormalCaptcha_ReturnsSolvedText()
        {
            GetCredentials(out var u, out var p);
            string imgPath = NormalCaptchaImagePath;
            Assert.True(File.Exists(imgPath),
                $"Test image not found at output path: {imgPath}");

            var client = new HttpClient(u, p) { Verbose = false };
            try
            {
                Captcha captcha = client.Decode(imgPath, DecodeTimeoutSeconds);

                Assert.NotNull(captcha);
                Assert.True(captcha.Id > 0,
                    $"Captcha.Id should be > 0, got {captcha.Id}");
                Assert.True(captcha.Solved,
                    $"Captcha {captcha.Id} should be marked Solved");
                Assert.True(captcha.Correct,
                    $"Captcha {captcha.Id} should be marked Correct");
                Assert.False(string.IsNullOrWhiteSpace(captcha.Text),
                    $"Captcha {captcha.Id} text should not be empty");

                Console.WriteLine(
                    $"[HTTP] Solved captcha id={captcha.Id} text='{captcha.Text}'");
            }
            finally { client.Close(); }
        }

        // ─── Socket client ──────────────────────────────────────────────────

        [SkippableFact]
        public void Socket_GetBalance_IsNonNegative()
        {
            GetCredentials(out var u, out var p);
            var client = new SocketClient(u, p) { Verbose = false };
            try
            {
                double balance = client.GetBalance();
                Assert.True(balance >= 0d,
                    $"Expected balance >= 0, got {balance}");
            }
            finally { client.Close(); }
        }

        [SkippableFact]
        public void Socket_GetUser_IsLoggedInWithPositiveId()
        {
            GetCredentials(out var u, out var p);
            var client = new SocketClient(u, p) { Verbose = false };
            try
            {
                User user = client.GetUser();

                Assert.NotNull(user);
                Assert.True(user.LoggedIn,
                    "User.LoggedIn should be true for valid credentials");
                Assert.True(user.Id > 0,
                    $"User.Id should be > 0, got {user.Id}");
                Assert.True(user.Balance >= 0d,
                    $"User.Balance should be >= 0, got {user.Balance}");

                Console.WriteLine(
                    $"[Socket] User id={user.Id} balance={user.Balance:F2} cents");
            }
            finally { client.Close(); }
        }

        [SkippableFact]
        public void Socket_DecodeNormalCaptcha_ReturnsSolvedText()
        {
            GetCredentials(out var u, out var p);
            string imgPath = NormalCaptchaImagePath;
            Assert.True(File.Exists(imgPath),
                $"Test image not found at output path: {imgPath}");

            var client = new SocketClient(u, p) { Verbose = false };
            try
            {
                Captcha captcha = client.Decode(imgPath, DecodeTimeoutSeconds);

                Assert.NotNull(captcha);
                Assert.True(captcha.Id > 0,
                    $"Captcha.Id should be > 0, got {captcha.Id}");
                Assert.True(captcha.Solved,
                    $"Captcha {captcha.Id} should be marked Solved");
                Assert.True(captcha.Correct,
                    $"Captcha {captcha.Id} should be marked Correct");
                Assert.False(string.IsNullOrWhiteSpace(captcha.Text),
                    $"Captcha {captcha.Id} text should not be empty");

                Console.WriteLine(
                    $"[Socket] Solved captcha id={captcha.Id} text='{captcha.Text}'");
            }
            finally { client.Close(); }
        }

        [SkippableFact]
        public void Http_FullApiSurface_BasicWorkflow()
        {
            GetCredentials(out var u, out var p);
            RequireFullIntegrationEnabled();
            var client = new HttpClient(u, p) { Verbose = false };
            try
            {
                RunFullApiSurface(client, "HTTP-FULL");
            }
            finally { client.Close(); }
        }

        [SkippableFact]
        public void Socket_FullApiSurface_BasicWorkflow()
        {
            GetCredentials(out var u, out var p);
            RequireFullIntegrationEnabled();
            var client = new SocketClient(u, p) { Verbose = false };
            try
            {
                RunFullApiSurface(client, "SOCKET-FULL");
            }
            finally { client.Close(); }
        }
    }
}
