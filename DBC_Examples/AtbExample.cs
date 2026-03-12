using System;
using System.Collections;
using DeathByCaptcha;

namespace DeathByCaptcha
{
    public class AtbExample
    {
        static public void Main(string[] argv)
        {
            // Put your DeathByCaptcha account username and password here.
            string username = "my_username";
            string password = "my_password";
            // string token_from_panel = "your-token-from-panel";

            /* Death By Captcha Socket Client
               Client client = (Client) new SocketClient(username, password);
               Death By Captcha http Client */
            Client client = (Client) new HttpClient(username, password);

            /* To use token authentication the first parameter must be "authtoken".
            Client client = (Client) new HttpClient("authtoken", token_from_panel); */

            // Put your Proxy credentials and type here
            // string proxy = "http://user:password@127.0.0.1:1234";
            // string proxyType = "HTTP";
            string proxy = "";
            string proxyType = "";
            string appid = "af23e041b22d000a11e22a230fa8991c";
            string pageurl = "https://www.example.com/";
            string apiserver = "https://cap.aisecurius.com";

            string tokenParams = "{\"proxy\": \"" + proxy + "\"," +
                                 "\"proxytype\": \"" + proxyType + "\"," +
                                 "\"appid\": \"" + appid + "\"," +
                                 "\"pageurl\": \"" + pageurl + "\"," +
                                 "\"apiserver\": \"" + apiserver + "\"}";

            try
            {
                double balance = client.GetBalance();

                /* Upload a CAPTCHA and poll for its status.  Put the Token CAPTCHA
                   Json payload, CAPTCHA type and desired solving timeout (in seconds)
                   here. If solved, you'll receive a DeathByCaptcha.Captcha object. */
                Captcha captcha = client.Decode(Client.DefaultTimeout,
                    new Hashtable()
                    {
                        {"type", 24},
                        {"atb_params", tokenParams}
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
