// cutcaptcha

using System;
using System.Collections;
using DeathByCaptcha;

namespace DeathByCaptcha
{
    public class CutcaptchaExample
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
            string apikey = "SAs61IAI";
            string miserykey = "56a9e9b989aa8cf99e0cea28d4b4678b84fa7a4e";
            string pageurl = "https://filecrypt.cc/Contact.html";

            string cutcaptchaParams = "{\"proxy\": \"" + proxy + "\"," +
                                    "\"proxytype\": \"" + proxyType + "\"," +
                                    "\"apikey\": \"" + apikey + "\"," +
                                    "\"miserykey\": \"" + miserykey + "\"," +
                                    "\"pageurl\": \"" + pageurl + "\"}";
            try
            {
                double balance = client.GetBalance();

                /* Upload a CAPTCHA and poll for its status. Put the cutcaptcha
                   Json payload, CAPTCHA type and desired solving timeout (in seconds)
                   here. If solved, you'll receive a DeathByCaptcha.Captcha object. */
                Captcha captcha = client.Decode(Client.DefaultTimeout,
                    new Hashtable()
                    {
                        {"type", 19},
                        {"cutcaptcha_params", cutcaptchaParams}
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
