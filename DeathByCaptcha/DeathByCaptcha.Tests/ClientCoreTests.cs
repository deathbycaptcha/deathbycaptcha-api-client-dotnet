using System;
using System.Collections;
using System.IO;
using System.Threading;
using DeathByCaptcha;
using Xunit;

namespace DeathByCaptcha.Tests;

public class ClientCoreTests
{
    [Fact]
    public void Credentials_UsesUsernameAndPassword_ByDefault()
    {
        var client = new FakeClient("alice", "secret");

        var credentials = client.Credentials;

        Assert.Equal("alice", credentials["username"]);
        Assert.Equal("secret", credentials["password"]);
        Assert.False(credentials.ContainsKey("authtoken"));
    }

    [Fact]
    public void Credentials_UsesAuthtoken_WhenUsernameIsAuthtoken()
    {
        var client = new FakeClient("authtoken", "token-value");

        var credentials = client.Credentials;

        Assert.Equal("token-value", credentials["authtoken"]);
        Assert.False(credentials.ContainsKey("username"));
        Assert.False(credentials.ContainsKey("password"));
    }

    [Fact]
    public void GetPollInterval_ReturnsDefault_WhenIndexOutOfRange()
    {
        Client.GetPollInterval(Client.LengthPollsInterval + 10, out var interval, out var nextIndex);

        Assert.Equal(Client.DefaultPollInterval, interval);
        Assert.Equal(Client.LengthPollsInterval + 11, nextIndex);
    }

    [Fact]
    public void GetPollInterval_ReturnsArrayValue_WhenIndexInRange()
    {
        Client.GetPollInterval(0, out var interval, out var nextIndex);

        Assert.Equal(Client.PollsInterval[0], interval);
        Assert.Equal(1, nextIndex);
    }

    [Fact]
    public void LoadData_ReturnsByteArray_AsIs()
    {
        var client = new FakeClient();
        var data = new byte[] { 1, 2, 3 };

        var loaded = client.LoadData(data);

        Assert.Equal(data, loaded);
    }

    [Fact]
    public void LoadStream_ReadsFromStart_AndRestoresPosition()
    {
        var client = new FakeClient();
        var data = new byte[] { 10, 11, 12, 13, 14 };
        using var stream = new MemoryStream(data);
        stream.Position = 2;

        var loaded = client.LoadStream(stream);

        Assert.Equal(data, loaded);
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    public void LoadData_InvalidType_ReturnsNull()
    {
        var client = new FakeClient();

        var loaded = client.LoadData(new object());

        Assert.Null(loaded);
    }

    [Fact]
    public void LoadFile_Throws_WhenMissing()
    {
        var client = new FakeClient();

        Assert.Throws<FileNotFoundException>(() => client.LoadFile("missing-captcha-file.bin"));
    }

    [Fact]
    public void Upload_Stream_UsesByteUploadPath()
    {
        var client = new FakeClient();
        using var stream = new MemoryStream(new byte[] { 9, 8, 7 });

        var captcha = client.Upload(stream);

        Assert.NotNull(captcha);
        Assert.Equal(new byte[] { 9, 8, 7 }, client.LastUploadedImage);
    }

    [Fact]
    public void Upload_File_UsesByteUploadPath()
    {
        var client = new FakeClient();
        var temp = Path.GetTempFileName();
        File.WriteAllBytes(temp, new byte[] { 4, 5, 6 });

        try
        {
            var captcha = client.Upload(temp);

            Assert.NotNull(captcha);
            Assert.Equal(new byte[] { 4, 5, 6 }, client.LastUploadedImage);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void GetCaptcha_And_GetText_Overloads_DelegateToIdVersion()
    {
        var client = new FakeClient();
        var byId = client.GetCaptcha(99);
        var byCaptcha = client.GetCaptcha(new Captcha(new Hashtable { { "captcha", 77 } }));
        var textById = client.GetText(88);
        var textByCaptcha = client.GetText(new Captcha(new Hashtable { { "captcha", 66 } }));

        Assert.Equal(99, byId.Id);
        Assert.Equal(77, byCaptcha.Id);
        Assert.Equal("resolved", textById);
        Assert.Equal("resolved", textByCaptcha);
    }

    [Fact]
    public void Report_Overload_DelegatesToIdVersion()
    {
        var client = new FakeClient();

        var ok = client.Report(new Captcha(new Hashtable { { "captcha", 12 } }));

        Assert.True(ok);
        Assert.Equal(12, client.LastReportedId);
    }

    [Fact]
    public void Decode_ByteArray_ReturnsSolvedCaptcha()
    {
        var client = new FakeClient
        {
            NextUploadCaptcha = new Captcha(new Hashtable
            {
                { "captcha", 123 },
                { "text", "done" },
                { "is_correct", true }
            })
        };

        var solved = client.Decode(new byte[] { 1, 2, 3 }, timeout: 1);

        Assert.NotNull(solved);
        Assert.Equal(123, solved.Id);
        Assert.Equal("done", solved.Text);
    }

    [Fact]
    public void Decode_ExtData_UsesUploadHashtablePath()
    {
        var client = new FakeClient
        {
            NextUploadExtCaptcha = new Captcha(new Hashtable
            {
                { "captcha", 222 },
                { "text", "token" },
                { "is_correct", true }
            })
        };

        var payload = new Hashtable { { "type", 4 }, { "token_params", "{}" } };

        var solved = client.Decode(timeout: 1, ext_data: payload);

        Assert.NotNull(solved);
        Assert.Equal(222, solved.Id);
        Assert.Same(payload, client.LastUploadExtData);
    }

    [Fact]
    public void Decode_Callback_InvokesDelegate()
    {
        var client = new FakeClient
        {
            NextUploadCaptcha = new Captcha(new Hashtable
            {
                { "captcha", 333 },
                { "text", "async" },
                { "is_correct", true }
            })
        };

        using var done = new ManualResetEventSlim(false);
        Captcha? callbackCaptcha = null;

        client.Decode(captcha =>
        {
            callbackCaptcha = captcha;
            done.Set();
        }, new byte[] { 6, 6, 6 }, timeout: 1);

        Assert.True(done.Wait(TimeSpan.FromSeconds(3)));
        Assert.NotNull(callbackCaptcha);
        Assert.Equal(333, callbackCaptcha.Id);
    }

    [Fact]
    public void Decode_Stream_And_File_Overloads_Work()
    {
        var client = new FakeClient
        {
            NextUploadCaptcha = new Captcha(new Hashtable
            {
                { "captcha", 444 },
                { "text", "stream-file" },
                { "is_correct", true }
            })
        };

        using var stream = new MemoryStream(new byte[] { 1, 9, 9 });
        var fromStream = client.Decode(stream, timeout: 1);

        var temp = Path.GetTempFileName();
        File.WriteAllBytes(temp, new byte[] { 2, 9, 9 });
        try
        {
            var fromFile = client.Decode(temp, timeout: 1);
            Assert.Equal(444, fromFile.Id);
        }
        finally
        {
            File.Delete(temp);
        }

        Assert.Equal(444, fromStream.Id);
    }

    [Fact]
    public void Decode_Callback_ForStream_AndFile_InvokesDelegate()
    {
        var client = new FakeClient
        {
            NextUploadCaptcha = new Captcha(new Hashtable
            {
                { "captcha", 555 },
                { "text", "callback-overloads" },
                { "is_correct", true }
            })
        };

        using var done1 = new ManualResetEventSlim(false);
        Captcha callback1 = null;
        using var stream = new MemoryStream(new byte[] { 7, 7, 7 });
        client.Decode(c =>
        {
            callback1 = c;
            done1.Set();
        }, stream, timeout: 1);
        Assert.True(done1.Wait(TimeSpan.FromSeconds(3)));
        Assert.NotNull(callback1);

        using var done2 = new ManualResetEventSlim(false);
        Captcha callback2 = null;
        var temp = Path.GetTempFileName();
        File.WriteAllBytes(temp, new byte[] { 8, 8, 8 });
        try
        {
            client.Decode(c =>
            {
                callback2 = c;
                done2.Set();
            }, temp, timeout: 1);
            Assert.True(done2.Wait(TimeSpan.FromSeconds(3)));
            Assert.NotNull(callback2);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void Balance_And_User_Properties_ForwardToGetUser()
    {
        var client = new FakeClient
        {
            NextUser = new User(new Hashtable
            {
                { "user", 99 },
                { "balance", 15.25 },
                { "is_banned", false }
            })
        };

        Assert.Equal(15.25, client.GetBalance(), 2);
        Assert.Equal(15.25, client.Balance, 2);
        Assert.Equal(99, client.User.Id);
    }
}
