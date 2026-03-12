// geetest

using System;
using System.Collections;
using DeathByCaptcha;

namespace DeathByCaptcha
{
    public class GeetestV3Example
    {
        static public void Main(string[] argv)
        {
            // Put your DeathByCaptcha account username and password here.
            string username = "username";
            string password = "password";
            // string token_from_panel = "your-token-from-panel";

            // Death By Captcha Socket Client
               Client client = (Client) new SocketClient(username, password);
            // Death By Captcha http Client
            // Client client = (Client) new HttpClient(username, password);

            /* To use token authentication the first parameter must be "authtoken".
            Client client = (Client) new HttpClient("authtoken", token_from_panel); */

            // Put your Proxy credentials and type here
            // string proxy = "http://user:password@127.0.0.1:1234";
            // string proxyType = "HTTP";
            string proxy = "";
            string proxyType = "";
            string gt = "022397c99c9f646f6477822485f30404";
            string challenge = "e90f426feadd58cddcac818444f48a93";
            string pageurl = "https://www.geetest.com/en/demo";

            // IMPORTANT: challenge parameter changes everytime
            // target site realoads the page
            // in this case we can see parameters here
            // https://www.geetest.com/demo/gt/register-enFullpage-official?t=1664547919370
            // just in this case, every site is different
            // we must examine the api calls to geetest to get the challenge

            string geetestParams = "{\"proxy\": \"" + proxy + "\"," +
                                    "\"proxytype\": \"" + proxyType + "\"," +
                                    "\"gt\": \"" + gt + "\"," +
                                    "\"challenge\": \"" + challenge + "\"," +
                                    "\"pageurl\": \"" + pageurl + "\"}";
            try
            {
                double balance = client.GetBalance();

                /* Upload a CAPTCHA and poll for its status. Put the geetest
                   Json payload, CAPTCHA type and desired solving timeout (in seconds)
                   here. If solved, you'll receive a DeathByCaptcha.Captcha object. */
                Captcha captcha = client.Decode(Client.DefaultTimeout,
                    new Hashtable()
                    {
                        {"type", 8},
                        {"geetest_params", geetestParams}
                    });

                if (null != captcha)
                {
                    /* The CAPTCHA was solved; captcha.Id property holds
                    its numeric ID, and captcha.Text holds its text. */
                    Console.WriteLine("CAPTCHA {0} solved: {1}", captcha.Id,
                        captcha.Text);

                    // // To access the response by item
                    // Hashtable text = (Hashtable) SimpleJson.Reader.Read(captcha.Text);
                    // Console.WriteLine("challenge: {0}", text["challenge"]);
                    // Console.WriteLine("validate: {0}", text["validate"]);
                    // Console.WriteLine("seccode: {0}", text["seccode"]);

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
