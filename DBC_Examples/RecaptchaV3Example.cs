// recaptcha_V3

using System;
using System.Collections;
using DeathByCaptcha;

namespace DeathByCaptcha
{
    public class RecaptchaV3Example
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
            string googlekey = "6LdyC2cUAAAAACGuDKpXeDorzUDWXmdqeg-xy696";
            string pageurl = "https://recaptchav3.demo.com/scores.php";

            /* - recaptcha_V3 requires 'action' that is the action that triggers
               recaptcha_V3 validation
               - if 'action' isn't' provided we use the default value "verify"
               - also you need to provide 'min_score', a number from 0.1 to 0.9,
               this is the minimum score acceptable from recaptchaV3 */
            string action = "example/action";
            double min_score = 0.3;


            string tokenParams = "{\"proxy\": \"" + proxy + "\"," +
                                 "\"proxytype\": \"" + proxyType + "\"," +
                                 "\"googlekey\": \"" + googlekey + "\"," +
                                 "\"pageurl\": \"" + pageurl + "\"," +
                                 "\"action\": \"" + action + "\"," +
                                 "\"min_score\": \"" + min_score + "\"}";

            try
            {
                double balance = client.GetBalance();

                /* Upload a CAPTCHA and poll for its status.  Put the Token CAPTCHA
                   Json payload, CAPTCHA type and desired solving timeout (in seconds)
                   here. If solved, you'll receive a DeathByCaptcha.Captcha object. */
                Captcha captcha = client.Decode(Client.DefaultTimeout,
                    new Hashtable()
                    {
                        {"type", 5},
                        {"token_params", tokenParams}
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
