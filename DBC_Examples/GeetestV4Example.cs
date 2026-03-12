// geetest

using System;
using System.Collections;
using DeathByCaptcha;

namespace DeathByCaptcha
{
    public class GeetestV4Example
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
            string captcha_id = "fcd636b4514bf7ac4143922550b3008b";
            string pageurl = "https://www.geetest.com/en/adaptive-captcha-demo";

            string geetestParams = "{\"proxy\": \"" + proxy + "\"," +
                                    "\"proxytype\": \"" + proxyType + "\"," +
                                    "\"captcha_id\": \"" + captcha_id + "\"," +
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
                        {"type", 9},
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
                    // Console.WriteLine("captcha_id: {0}", text["captcha_id"]);
                    // Console.WriteLine("lot_number: {0}", text["lot_number"]);
                    // Console.WriteLine("pass_token: {0}", text["pass_token"]);
                    // Console.WriteLine("gen_time: {0}", text["gen_time"]);
                    // Console.WriteLine("captcha_output: {0}", text["captcha_output"]);

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
