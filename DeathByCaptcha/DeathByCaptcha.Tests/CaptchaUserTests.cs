using System.Collections;
using DeathByCaptcha;
using Xunit;

namespace DeathByCaptcha.Tests;

public class CaptchaUserTests
{
    [Fact]
    public void Captcha_DefaultState_IsEmpty()
    {
        var captcha = new Captcha();

        Assert.Equal(0, captcha.Id);
        Assert.False(captcha.Uploaded);
        Assert.False(captcha.Solved);
        Assert.False(captcha.Correct);
        Assert.Null(captcha.Text);
    }

    [Fact]
    public void Captcha_IdSetter_ResetsTextAndCorrectness()
    {
        var captcha = new Captcha(new Hashtable
        {
            { "captcha", 10 },
            { "text", "abc" },
            { "is_correct", true }
        });

        captcha.Id = 5;

        Assert.Equal(5, captcha.Id);
        Assert.Null(captcha.Text);
        Assert.False(captcha.Correct);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Captcha_TextSetter_NormalizesEmptyToNull(string? value)
    {
        var captcha = new Captcha();

        captcha.Text = value;

        Assert.Null(captcha.Text);
    }

    [Fact]
    public void Captcha_CanParseJsonLikeTextPayload()
    {
        var nested = new Hashtable { { "x", 1 }, { "y", 2 } };
        var src = new Hashtable
        {
            { "captcha", 7 },
            { "text", nested },
            { "is_correct", true }
        };

        var captcha = new Captcha(src);

        Assert.True(captcha.Uploaded);
        Assert.True(captcha.Solved);
        Assert.True(captcha.Correct);
        Assert.Contains("x", captcha.Text);
        Assert.Equal(7, captcha.ToInt());
        Assert.Equal(captcha.Text, captcha.ToString());
        Assert.True(captcha.ToBoolean());
    }

    [Fact]
    public void User_DefaultState_IsLoggedOut()
    {
        var user = new User();

        Assert.Equal(0, user.Id);
        Assert.False(user.LoggedIn);
        Assert.Equal(0.0, user.Balance);
        Assert.False(user.Banned);
        Assert.False(user.ToBoolean());
    }

    [Fact]
    public void User_ParsesValues_FromHashtable()
    {
        var user = new User(new Hashtable
        {
            { "user", 100 },
            { "balance", 42.75 },
            { "is_banned", true }
        });

        Assert.Equal(100, user.Id);
        Assert.True(user.LoggedIn);
        Assert.Equal(42.75, user.Balance, 3);
        Assert.True(user.Banned);
        Assert.False(user.ToBoolean());
    }

    [Fact]
    public void User_IdSetter_ResetsBalanceAndBanned()
    {
        var user = new User(new Hashtable
        {
            { "user", 100 },
            { "balance", 42.75 },
            { "is_banned", true }
        });

        user.Id = 5;

        Assert.Equal(5, user.Id);
        Assert.Equal(0.0, user.Balance);
        Assert.False(user.Banned);
    }
}
