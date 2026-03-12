// Amazon WAF

using System;
using System.Collections;
using DeathByCaptcha;

namespace DeathByCaptcha
{
    public class AmazonWafExample
    {
        static public void Main(string[] argv)
        {
            // Put your DeathByCaptcha account username and password here.
            string username = "your username";
            string password = "your password";
            // string token_from_panel = "your-token-from-panel";

            /* Death By Captcha Socket Client
               Client client = (Client) new SocketClient(username, password);
               Death By Captcha http Client */
            Client client = (Client) new HttpClient(username, password);

            /* To use token authentication the first parameter must be "authtoken".
            Client client = (Client) new HttpClient("authtoken", token_from_panel); */

            // Put your Proxy credentials and type here
            string proxy = "http://user:password@127.0.0.1:1234";
            string proxyType = "HTTP";
            string sitekey = "AQIDAHjcYu/GjX+QlghicBgQ/7bFaQZ+m5FKCMDnO+vTbNg96AHDh0IR5vgzHNceHYqZR+GOAAAAfjB8BgkqhkiG9w0BBwagbzBtAgEAMGgGCSqGSIb3DQEHATAeBglghkgBZQMEAS4wEQQMsYNQbVOLOfd/1ofjAgEQgDuhVKc2V/0XTEPc+9X/xAodxDqgyNNNyYJN1rM2gs4yBMeDXXc3z2ZxmD9jsQ8eNMGHqeii56iL2Guh4A==";
            string pageurl = "https://efw47fpad9.execute-api.us-east-1.amazonaws.com/latest";
            string iv = "CgAFRjIw2vAAABSM";
            string context = "zPT0jOl1rQlUNaldX6LUpn4D6Tl9bJ8VUQ/NrWFxPiiFujn5bFHzpOlKYQG0Di/UrO/p0xItkf7oGrknHqnj+UjvWv+i0BFbm3vGKceNaGtjrg4wvydL2Li5XjwRUOMW4o+NgO3JPJhkgwRKSyK62cIIzrThlOBD+gmtvKW0JNtH8efKR8Y5mBf0gi8JokjUxq/XbyB6h83tfaiWrp3dkOJsEXHLkT/wwQlFZysA919LCA+XVqgJ9lurUZqHWar+9JHqWnc0ghckKCnUzubvSQzJl+eSIAIoYZrpuZQszOwWzo4=";
            string challengejs = ""; // optional parameter
            string captchajs = "";   // optional parameter

            string wafParams = "{\"proxy\": \"" + proxy + "\"," +
                                    "\"proxytype\": \"" + proxyType + "\"," +
                                    "\"sitekey\": \"" + sitekey + "\"," +
                                    "\"pageurl\": \"" + pageurl + "\"," +
                                    "\"iv\": \"" + iv + "\"," +
                                    "\"context\": \"" + context + "\"}";
            try
            {
                double balance = client.GetBalance();

                /* Upload a CAPTCHA and poll for its status. Put the Amazon Waf
                   Json payload and CAPTCHA type
                   here. If solved, you'll receive a DeathByCaptcha.Captcha object. */
                Captcha captcha = client.Decode(Client.DefaultTimeout,
                    new Hashtable()
                    {
                        {"type", 16},
                        {"waf_params", wafParams}
                    });

                if (null != captcha)
                {
                    /* The CAPTCHA was solved; captcha.Id property holds
                    its numeric ID, and captcha.Text holds its text. */
                    Console.WriteLine("CAPTCHA {0} solved: {1}", captcha.Id,
                        captcha.Text);

//                  if ( /* check if the CAPTCHA was incorrectly solved */)
//                  {
//                      client.Report(captcha);
//                  }
                }
            }
            catch (AccessDeniedException e)
            {
                /* Access to DBC API denied, check your credentials and/or balance */
                Console.WriteLine("<<< catch : " + e.ToString());
            }
        }
    }
}
