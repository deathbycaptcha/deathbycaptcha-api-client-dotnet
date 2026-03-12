using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using DeathByCaptcha;
using Xunit;

namespace DeathByCaptcha.Tests;

public class HttpClientTests
{
    private sealed class TestHttpClient : DeathByCaptcha.HttpClient
    {
        private readonly string _baseUrl;
        private readonly Queue<HttpResponseMessage> _responses;

        public TestHttpClient(string baseUrl, params HttpResponseMessage[] responses) : base("user", "pass")
        {
            _baseUrl = baseUrl;
            _responses = new Queue<HttpResponseMessage>(responses);
            Verbose = false;
        }

        protected override string BaseApiUrl => _baseUrl;

        protected override HttpResponseMessage SendRequest(HttpRequestMessage request, TimeSpan timeout)
        {
            if (_responses.Count == 0)
            {
                throw new IOException("No queued responses for test client.");
            }

            return _responses.Dequeue();
        }
    }

    [Fact]
    public void GetUser_ReturnsParsedUser_On200()
    {
        var client = new TestHttpClient(
            "http://unit.test/api",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"user\":123,\"balance\":42.5,\"is_banned\":false}", Encoding.UTF8, "application/json")
            }
        );

        var user = client.GetUser();

        Assert.Equal(123, user.Id);
        Assert.Equal(42.5, user.Balance, 2);
        Assert.False(user.Banned);
    }

    [Fact]
    public void GetCaptcha_FollowsSeeOther_Redirect()
    {
        var redirect = new HttpResponseMessage(HttpStatusCode.SeeOther);
        redirect.Headers.Location = new Uri("/api/captcha/9/final", UriKind.Relative);

        var client = new TestHttpClient(
            "http://unit.test/api",
            redirect,
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"captcha\":9,\"text\":\"done\",\"is_correct\":true}", Encoding.UTF8, "application/json")
            }
        );

        var captcha = client.GetCaptcha(9);

        Assert.Equal(9, captcha.Id);
        Assert.Equal("done", captcha.Text);
        Assert.True(captcha.Correct);
    }

    [Fact]
    public void GetUser_ThrowsAccessDenied_On403()
    {
        var client = new TestHttpClient(
            "http://unit.test/api",
            new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            }
        );

        Assert.Throws<AccessDeniedException>(() => client.GetUser());
    }

    [Fact]
    public void GetCaptcha_ThrowsInvalidCaptcha_On400()
    {
        var client = new TestHttpClient(
            "http://unit.test/api",
            new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            }
        );

        Assert.Throws<InvalidCaptchaException>(() => client.GetCaptcha(1));
    }

    [Fact]
    public void GetCaptcha_ThrowsServiceOverload_On503()
    {
        var client = new TestHttpClient(
            "http://unit.test/api",
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            }
        );

        Assert.Throws<ServiceOverloadException>(() => client.GetCaptcha(2));
    }

    [Fact]
    public void Upload_ReturnsUploadedCaptcha_On200()
    {
        var client = new TestHttpClient(
            "http://unit.test/api",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"captcha\":11,\"text\":\"ok\",\"is_correct\":true}", Encoding.UTF8, "application/json")
            }
        );

        var captcha = client.Upload(new byte[] { 1, 2, 3 }, new Hashtable { { "type", 2 } });

        Assert.NotNull(captcha);
        Assert.Equal(11, captcha.Id);
        Assert.Equal("ok", captcha.Text);
    }

    [Fact]
    public void Report_ZeroId_ReturnsFalse_WithoutNetwork()
    {
        var client = new TestHttpClient("http://unit.test/api");

        var result = client.Report(0);

        Assert.False(result);
    }
}
