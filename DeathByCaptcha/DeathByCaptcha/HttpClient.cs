/**
 */

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;


namespace DeathByCaptcha
{
    /**
     * <summary>Death by Captcha HTTP API client.</summary>
     */
    public class HttpClient : Client
    {
        /**
         * <value>Base URL for API calls.</value>
         */
        public const string ServerUrl = "https://api.dbcapi.me/api";

        /**
         * <value>Desired API response MIME type.</value>
         */
        public const string ResponseContentType = "application/json";

        /**
         * <value>HTTP proxy to use, if necessary.</value>
         */
        public WebProxy Proxy = null;

        /**
         * <value>When set to true, the client will accept any TLS server certificate for this instance only.
         * This is unsafe and should only be used in controlled environments.</value>
         */
        public bool AcceptAnyServerCertificate = false;


        private System.Net.Http.HttpClient _httpClient = null;
        private WebProxy _httpClientProxy = null;
        private bool _httpClientAcceptAnyServerCertificate = false;

        protected virtual string BaseApiUrl
        {
            get
            {
                return ServerUrl;
            }
        }

        private System.Net.Http.HttpClient GetOrCreateHttpClient()
        {
            if (null == _httpClient
                || !Object.ReferenceEquals(_httpClientProxy, this.Proxy)
                || _httpClientAcceptAnyServerCertificate != this.AcceptAnyServerCertificate)
            {
                if (null != _httpClient)
                {
                    _httpClient.Dispose();
                }

                HttpClientHandler handler = new HttpClientHandler();
                handler.AllowAutoRedirect = false;
                handler.UseProxy = false;

                if (null != this.Proxy)
                {
                    handler.Proxy = this.Proxy;
                    handler.UseProxy = true;
                }

                if (this.AcceptAnyServerCertificate)
                {
                    handler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }

                _httpClient = new System.Net.Http.HttpClient(handler);
                _httpClient.Timeout = Timeout.InfiniteTimeSpan;
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.ParseAdd(HttpClient.ResponseContentType);
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(Client.Version);

                _httpClientProxy = this.Proxy;
                _httpClientAcceptAnyServerCertificate = this.AcceptAnyServerCertificate;
            }

            return _httpClient;
        }

        protected virtual HttpResponseMessage SendRequest(HttpRequestMessage request, TimeSpan timeout)
        {
            System.Net.Http.HttpClient httpClient = this.GetOrCreateHttpClient();
            using (CancellationTokenSource cts = new CancellationTokenSource(timeout))
            {
                return httpClient.SendAsync(request, cts.Token).GetAwaiter().GetResult();
            }
        }

        protected string SendReceive(string cmd, byte[] payload, string contentType)
        {
            string response = null;

            DateTime deadline = DateTime.Now.AddSeconds(Client.DefaultTimeout);
            string url = this.BaseApiUrl + "/" + cmd;
            while (deadline > DateTime.Now && null != url)
            {
                string requestUrl = url;
                url = null;

                this.Log("SEND", requestUrl.ToString());

                using (HttpRequestMessage request = new HttpRequestMessage(0 < payload.Length
                    ? HttpMethod.Post
                    : HttpMethod.Get,
                    requestUrl))
                {
                    request.Headers.ExpectContinue = false;

                    if (0 < payload.Length)
                    {
                        request.Content = new ByteArrayContent(payload);
                        request.Content.Headers.TryAddWithoutValidation("Content-Type", contentType);
                    }

                    HttpResponseMessage httpResponse;
                    try
                    {
                        httpResponse = this.SendRequest(
                            request,
                            TimeSpan.FromMilliseconds(Math.Max(1, (deadline - DateTime.Now).TotalMilliseconds))
                        );
                    }
                    catch (OperationCanceledException)
                    {
                        return response;
                    }
                    catch (System.Exception e)
                    {
                        throw new IOException("API connection failed", e);
                    }

                    using (httpResponse)
                    {
                        if (HttpStatusCode.Forbidden == httpResponse.StatusCode)
                        {
                            throw new AccessDeniedException(
                                "Access denied, check your credentials and/or balance"
                            );
                        }
                        else if (HttpStatusCode.BadRequest == httpResponse.StatusCode)
                        {
                            throw new InvalidCaptchaException(
                                "CAPTCHA was rejected, please check if it's a valid image"
                            );
                        }
                        else if (HttpStatusCode.ServiceUnavailable == httpResponse.StatusCode)
                        {
                            throw new ServiceOverloadException(
                                "CAPTCHA was rejected due to service overload, try again later"
                            );
                        }
                        else if (HttpStatusCode.SeeOther == httpResponse.StatusCode)
                        {
                            url = null;
                            if (null != httpResponse.Headers.Location)
                            {
                                url = httpResponse.Headers.Location.IsAbsoluteUri
                                    ? httpResponse.Headers.Location.ToString()
                                    : new Uri(new Uri(this.BaseApiUrl + "/"), httpResponse.Headers.Location).ToString();
                            }
                        }
                        else
                        {
                            // Some API responses use non-standard charset tokens (e.g. utf8),
                            // which can fail with ReadAsStringAsync on .NET. Decode as UTF-8 bytes.
                            byte[] responseBytes = httpResponse.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                            response = Encoding.UTF8.GetString(responseBytes);
                            this.Log("RECV", response);
                        }
                    }
                }

                payload = new byte[0];
            }

            return response;
        }

        protected Hashtable Call(string cmd, byte[] payload, string contentType)
        {
            if (contentType.Equals(String.Empty))
            {
                contentType = "application/x-www-form-urlencoded";
            }

            string buf = null;
            lock (this._callLock)
            {
                buf = this.SendReceive(cmd, payload, contentType);
            }

            if (null != buf)
            {
                try
                {
                    return (Hashtable) SimpleJson.Reader.Read(buf);
                }
                catch (FormatException)
                {
                    return new Hashtable();
                }
            }
            else
            {
                return null;
            }
        }

        protected Hashtable Call(string cmd, Hashtable args)
        {
            string[] fields = new string[args.Count];
            int i = 0;
            foreach (DictionaryEntry e in args)
            {
                fields[i] = HttpUtility.UrlEncode((string) e.Key) + "=" + HttpUtility.UrlEncode((string) e.Value);
                i++;
            }

            return this.Call(cmd,
                Encoding.ASCII.GetBytes(String.Join("&", fields)),
                String.Empty);
        }

        protected Hashtable Call(string cmd)
        {
            return this.Call(cmd, new Hashtable());
        }


        /**
         * <see cref="M:DeathByCaptcha.Client(String, String)"/>
         */
        public HttpClient(string username, string password) : base(username, password)
        {
        }


        /**
         * <see cref="M:DeathByCaptcha.Client.Close()"/>
         */
        public override void Close()
        {
            lock (this._callLock)
            {
                if (null != _httpClient)
                {
                    _httpClient.Dispose();
                    _httpClient = null;
                    _httpClientProxy = null;
                    _httpClientAcceptAnyServerCertificate = false;
                }
            }
        }


        /**
         * <see cref="M:DeathByCaptcha.Client.GetUser()"/>
         */
        public override User GetUser()
        {
            return new User(this.Call("user", this.Credentials));
        }

        /**
         * <see cref="M:DeathByCaptcha.Client.GetCaptcha(int)"/>
         */
        public override Captcha GetCaptcha(int id)
        {
            return new Captcha(this.Call("captcha/" + id));
        }

        /**
         * <see cref="M:DeathByCaptcha.Client.Upload(byte[])"/>
         * <param name="ext_data">Extra data used by special captchas types.</param>
         */
        public override Captcha Upload(byte[] img, Hashtable ext_data = null)
        {
            string boundary = Convert.ToHexString(
                SHA1.HashData(Encoding.ASCII.GetBytes(DateTime.Now.ToString("G")))
            );

            Hashtable args = this.Credentials;
            args["swid"] = Convert.ToString(Client.SoftwareVendorId);

            byte[] banner = null;
            if (ext_data != null)
            {
                foreach (DictionaryEntry item in ext_data)
                {
                    if (Convert.ToString(item.Key) == "banner")
                    {
                        banner = this.Load(ext_data["banner"]);
                    }
                    else
                    {
                        args[Convert.ToString(item.Key)] = item.Value.ToString();
                    }
                }
            }

            string[] rawArgs = new string[args.Count + 2];
            int i = 0;
            foreach (DictionaryEntry e in args)
            {
                string v = (string) e.Value;
                rawArgs[i++] = String.Join("\r\n", new string[]
                {
                    "--" + boundary,
                    "Content-Disposition: form-data; name=\"" + (string) e.Key + "\"",
                    "Content-Length: " + v.Length,
                    "",
                    v
                });
            }

            rawArgs[i++] = String.Join("\r\n", new string[]
            {
                "--" + boundary,
                "Content-Disposition: form-data; name=\"captchafile\"; filename=\"captcha.jpeg\"",
                "Content-Type: application/octet-stream",
                "Content-Length: " + img.Length,
                ""
            });

            string banner_hdr = "";
            int banner_len = 0;
            byte[] b_hdr = new byte[] { };
            if (banner != null)
            {
                banner_hdr = String.Join("\r\n", new string[]
                {
                    "\r\n--" + boundary,
                    "Content-Disposition: form-data; name=\"banner\"; filename=\"banner.jpeg\"",
                    "Content-Type: application/octet-stream",
                    "Content-Length: " + banner.Length,
                    "\r\n"
                });
                b_hdr = Encoding.ASCII.GetBytes(banner_hdr);
                banner_len = b_hdr.Length + banner.Length;
            }
            else
            {
                banner = new byte[] { };
            }

            byte[] hdr = Encoding.ASCII.GetBytes(String.Join("\r\n", rawArgs));
            byte[] ftr = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            byte[] body = new byte[hdr.Length + img.Length + banner_len + ftr.Length];
            hdr.CopyTo(body, 0);
            img.CopyTo(body, hdr.Length);

            if (banner_len > 0)
            {
                b_hdr.CopyTo(body, hdr.Length + img.Length);
                banner.CopyTo(body, hdr.Length + img.Length + b_hdr.Length);
            }

            ftr.CopyTo(body, hdr.Length + img.Length + banner_len);

            Captcha c = new Captcha(this.Call(
                "captcha",
                body,
                "multipart/form-data; boundary=" + boundary
            ));
            return c.Uploaded ? c : null;
        }


        /**
         * <see cref="M:DeathByCaptcha.Client.Upload(ext_data[])"/>
         * <param name="ext_data">Extra data used by special captchas types.</param>
         */
        public override Captcha Upload(Hashtable ext_data)
        {
            string boundary = Convert.ToHexString(
                SHA1.HashData(Encoding.ASCII.GetBytes(DateTime.Now.ToString("G")))
            );

            Hashtable args = this.Credentials;
            args["swid"] = Convert.ToString(Client.SoftwareVendorId);


            if (ext_data != null)
            {
                foreach (DictionaryEntry item in ext_data)
                {
                    args[Convert.ToString(item.Key)] = item.Value.ToString();
                }
            }

            string[] rawArgs = new string[args.Count + 2];
            int i = 0;
            foreach (DictionaryEntry e in args)
            {
                string v = (string) e.Value;
                rawArgs[i++] = String.Join("\r\n", new string[]
                {
                    "--" + boundary,
                    "Content-Disposition: form-data; name=\"" + (string) e.Key + "\"",
                    "Content-Length: " + v.Length,
                    "",
                    v
                });
            }

            byte[] hdr = Encoding.ASCII.GetBytes(String.Join("\r\n", rawArgs));
            byte[] ftr = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            byte[] body = new byte[hdr.Length + ftr.Length];
            hdr.CopyTo(body, 0);

            ftr.CopyTo(body, hdr.Length);

            Captcha c = new Captcha(this.Call(
                "captcha",
                body,
                "multipart/form-data; boundary=" + boundary
            ));
            return c.Uploaded ? c : null;
        }

        /**
         * <see cref="M:DeathByCaptcha.Client.Report(int)"/>
         */
        public override bool Report(int id)
        {
            return 0 < id && !(new Captcha(this.Call("captcha/" + id + "/report",
                       this.Credentials))).Correct;
        }
    }
}
