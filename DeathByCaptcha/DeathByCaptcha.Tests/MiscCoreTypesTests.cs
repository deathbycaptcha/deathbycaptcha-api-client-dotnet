using DeathByCaptcha;
using Xunit;

namespace DeathByCaptcha.Tests;

public class MiscCoreTypesTests
{
    [Fact]
    public void PollPayload_StoresValues()
    {
        var payload = new PollPayload();
        Captcha received = null;
        payload.Callback = c => received = c;
        payload.Captcha = new Captcha();
        payload.Timeout = 9;

        payload.Callback(payload.Captcha);

        Assert.Same(payload.Captcha, received);
        Assert.Equal(9, payload.Timeout);
    }

    [Fact]
    public void CustomExceptions_ExposeMessage()
    {
        var denied = new AccessDeniedException("denied");
        var invalid = new InvalidCaptchaException("invalid");
        var overload = new ServiceOverloadException("overload");

        Assert.Equal("denied", denied.Message);
        Assert.Equal("invalid", invalid.Message);
        Assert.Equal("overload", overload.Message);
    }

    [Fact]
    public void DefaultExceptionConstructors_AreUsable()
    {
        Assert.NotNull(new AccessDeniedException());
        Assert.NotNull(new InvalidCaptchaException());
        Assert.NotNull(new ServiceOverloadException());
    }

    [Fact]
    public void UserToInt_ReturnsId()
    {
        var user = new User(new System.Collections.Hashtable
        {
            { "user", 321 },
            { "balance", 1.0 },
            { "is_banned", false }
        });

        Assert.Equal(321, user.ToInt());
    }
}
