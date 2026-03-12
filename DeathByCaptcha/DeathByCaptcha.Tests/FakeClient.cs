using System;
using System.Collections;
using System.IO;
using DeathByCaptcha;

namespace DeathByCaptcha.Tests;

internal sealed class FakeClient : Client
{
    public User NextUser { get; set; } = new(new Hashtable { { "user", 1 }, { "balance", 12.5 }, { "is_banned", false } });
    public Captcha NextUploadCaptcha { get; set; } = new(new Hashtable { { "captcha", 1 }, { "text", "ok" }, { "is_correct", true } });
    public Captcha NextUploadExtCaptcha { get; set; } = new(new Hashtable { { "captcha", 2 }, { "text", "ext" }, { "is_correct", true } });
    public Func<int, Captcha>? GetCaptchaFunc { get; set; }
    public int LastReportedId { get; private set; }
    public int LastGetCaptchaId { get; private set; }
    public byte[]? LastUploadedImage { get; private set; }
    public Hashtable? LastUploadExtData { get; private set; }

    public FakeClient(string username = "user", string password = "pass") : base(username, password)
    {
        Verbose = false;
    }

    public override void Close()
    {
    }

    public override User GetUser()
    {
        return NextUser;
    }

    public override Captcha GetCaptcha(int id)
    {
        LastGetCaptchaId = id;
        if (GetCaptchaFunc is not null)
        {
            return GetCaptchaFunc(id);
        }

        return new Captcha(new Hashtable { { "captcha", id }, { "text", "resolved" }, { "is_correct", true } });
    }

    public override Captcha Upload(byte[] img, Hashtable ext_data = null)
    {
        LastUploadedImage = img;
        LastUploadExtData = ext_data;
        return NextUploadCaptcha;
    }

    public override Captcha Upload(Hashtable ext_data)
    {
        LastUploadExtData = ext_data;
        return NextUploadExtCaptcha;
    }

    public override bool Report(int id)
    {
        LastReportedId = id;
        return id > 0;
    }

    public byte[] LoadData(object data) => Load(data);

    public byte[] LoadStream(Stream stream) => Load(stream);

    public byte[] LoadFile(string fileName) => Load(fileName);
}
