// new recaptcha image group

using System;
using System.Collections;
using DeathByCaptcha;

namespace DeathByCaptcha
{
    public class RecaptchaImageGroupExample
    {
        static public void Main(string[] argv)
        {
            // Put your DeathByCaptcha account username and password here.
            string username = "your username";
            string password = "your password";
            // string token_from_panel = "your-token-from-panel";

            string filename = "./images/test2.jpg";
            string banner = "./images/banner.jpg";
            string banner_text = "choose all pizza:";

            /* Death By Captcha Socket Client
               Client client = (Client) new SocketClient(username, password);
               Death By Captcha http Client */
            Client client = (Client) new HttpClient(username, password);

            /* To use token authentication the first parameter must be "authtoken".
            Client client = (Client) new HttpClient("authtoken", token_from_panel); */

            try
            {
                double balance = client.GetBalance();

                /* Put your CAPTCHA file name, or file object,
                   or arbitrary stream, or an array of bytes, 
                   and optional solving timeout (in seconds) here: 
                */
                Captcha captcha = client.Decode(filename, 0, new Hashtable()
                {
                    {"type", 3},
                    {"banner_text", banner_text},
                    {"banner", banner}
                    /*
                    - If you want to specify grid data: {"grid", "4x2"}
                    - If you wont supply  grid parameter, dbc would attempt to
                    autodetect proper grid.*/
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
