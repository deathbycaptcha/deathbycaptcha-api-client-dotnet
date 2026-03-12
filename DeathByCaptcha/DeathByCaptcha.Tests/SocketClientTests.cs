using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DeathByCaptcha;
using Xunit;

namespace DeathByCaptcha.Tests;

public class SocketClientTests
{
    private sealed class TestSocketClient : SocketClient
    {
        private readonly Queue<string> _responses = new();
        private readonly bool _connectResult;

        public List<string> SentPayloads { get; } = new();

        public TestSocketClient(bool connectResult = true) : base("user", "pass")
        {
            _connectResult = connectResult;
            Verbose = false;
        }

        public void QueueResponse(string json)
        {
            _responses.Enqueue(json);
        }

        protected override bool Connect()
        {
            return _connectResult;
        }

        protected override SocketClient Send(byte[] buf)
        {
            SentPayloads.Add(Encoding.ASCII.GetString(buf));
            return this;
        }

        protected override string Receive()
        {
            if (_responses.Count == 0)
            {
                throw new IOException("No queued response.");
            }

            return _responses.Dequeue();
        }

        protected override void Disconnect()
        {
        }
    }

    private sealed class ExposedSocketClient : SocketClient
    {
        public ExposedSocketClient() : base("user", "pass")
        {
            Verbose = false;
        }

        public void Attach(TcpClient client)
        {
            tcpClient = client;
        }

        public SocketClient InvokeSend(byte[] data)
        {
            return base.Send(data);
        }

        public string InvokeReceive()
        {
            return base.Receive();
        }

        public void InvokeDisconnect()
        {
            base.Disconnect();
        }
    }

    [Fact]
    public void GetUser_ReturnsParsedUser()
    {
        var client = new TestSocketClient();
        client.QueueResponse("{}");
        client.QueueResponse("{\"user\":55,\"balance\":10.5,\"is_banned\":false}");

        var user = client.GetUser();

        Assert.Equal(55, user.Id);
        Assert.Equal(10.5, user.Balance, 2);
        Assert.False(user.Banned);
    }

    [Fact]
    public void GetCaptcha_ReturnsParsedCaptcha()
    {
        var client = new TestSocketClient();
        client.QueueResponse("{}");
        client.QueueResponse("{\"captcha\":77,\"text\":\"solved\",\"is_correct\":true}");

        var captcha = client.GetCaptcha(77);

        Assert.Equal(77, captcha.Id);
        Assert.Equal("solved", captcha.Text);
        Assert.True(captcha.Correct);
    }

    [Fact]
    public void Upload_WithExtData_ReturnsCaptcha()
    {
        var client = new TestSocketClient();
        client.QueueResponse("{}");
        client.QueueResponse("{\"captcha\":99,\"text\":\"ok\",\"is_correct\":true}");

        var ext = new Hashtable { { "type", 2 }, { "foo", "bar" } };
        var captcha = client.Upload(new byte[] { 1, 2, 3 }, ext);

        Assert.NotNull(captcha);
        Assert.Equal(99, captcha.Id);
        Assert.Contains("\"cmd\":\"upload\"", string.Join("", client.SentPayloads));
    }

    [Fact]
    public void Upload_WithBannerAndUploadHashtable_AreCovered()
    {
        var client = new TestSocketClient();

        client.QueueResponse("{}");
        client.QueueResponse("{\"captcha\":101,\"text\":\"banner\",\"is_correct\":true}");

        var bannerFile = Path.GetTempFileName();
        File.WriteAllBytes(bannerFile, new byte[] { 10, 20, 30 });

        try
        {
            var ext = new Hashtable
            {
                { "type", 3 },
                { "banner", bannerFile },
                { "banner_text", "pick all" }
            };

            var first = client.Upload(new byte[] { 1, 2, 3 }, ext);
            Assert.NotNull(first);

            client.QueueResponse("{}");
            client.QueueResponse("{\"captcha\":102,\"text\":\"token\",\"is_correct\":true}");
            var second = client.Upload(new Hashtable { { "type", 4 }, { "token_params", "{}" } });
            Assert.NotNull(second);
            Assert.Equal(102, second.Id);
        }
        finally
        {
            File.Delete(bannerFile);
        }
    }

    [Fact]
    public void Report_ReturnsFalse_WhenCaptchaStillCorrect()
    {
        var client = new TestSocketClient();
        client.QueueResponse("{}");
        client.QueueResponse("{\"captcha\":10,\"text\":\"x\",\"is_correct\":true}");

        var result = client.Report(10);

        Assert.False(result);
    }

    [Theory]
    [InlineData("not-logged-in", typeof(AccessDeniedException))]
    [InlineData("banned", typeof(AccessDeniedException))]
    [InlineData("insufficient-funds", typeof(AccessDeniedException))]
    [InlineData("invalid-captcha", typeof(InvalidCaptchaException))]
    [InlineData("service-overload", typeof(ServiceOverloadException))]
    [InlineData("other-error", typeof(IOException))]
    public void ErrorResponses_MapToExpectedExceptions(string errorCode, Type expected)
    {
        var client = new TestSocketClient();
        client.QueueResponse("{}");
        client.QueueResponse($"{{\"error\":\"{errorCode}\"}}");

        var ex = Record.Exception(() => client.GetUser());

        Assert.NotNull(ex);
        Assert.IsType(expected, ex);
    }

    [Fact]
    public void ThrowsIOException_WhenConnectionFails()
    {
        var client = new TestSocketClient(connectResult: false);

        Assert.Throws<IOException>(() => client.GetUser());
    }

    [Fact]
    public void HandlesMalformedJson_AsEmptyResponse()
    {
        var client = new TestSocketClient();
        client.QueueResponse("{}");
        client.QueueResponse("{not-json");

        var user = client.GetUser();

        Assert.Equal(0, user.Id);
        Assert.False(user.LoggedIn);
    }

    [Fact]
    public void BaseSendReceiveAndDisconnect_WorkWithLoopback()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        using var serverSideAccepted = listener.AcceptTcpClientAsync();
        var clientSocket = new TcpClient();
        clientSocket.Connect(IPAddress.Loopback, port);
        var serverSocket = serverSideAccepted.GetAwaiter().GetResult();

        try
        {
            var client = new ExposedSocketClient();
            client.Attach(clientSocket);

            var payload = Encoding.ASCII.GetBytes("{\"x\":1}\r\n");
            _ = client.InvokeSend(payload);

            using var serverStream = serverSocket.GetStream();
            var readBuffer = new byte[payload.Length];
            var read = serverStream.Read(readBuffer, 0, readBuffer.Length);
            Assert.True(read > 0);

            var response = Encoding.ASCII.GetBytes("{\"ok\":true}\r\n");
            serverStream.Write(response, 0, response.Length);
            serverStream.Flush();

            var received = client.InvokeReceive();
            Assert.Contains("ok", received);

            client.Close();
            client.InvokeDisconnect();
        }
        finally
        {
            serverSocket.Close();
            listener.Stop();
        }
    }
}
