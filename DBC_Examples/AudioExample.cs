// audio

using System;
using System.IO;
using System.Collections;
using DeathByCaptcha;

namespace DeathByCaptcha
{
    public class AudioExample
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

            // Read the audio file and convert it to base64 string
            string base64String = null;
            try
            {
                byte[] fileBytes = File.ReadAllBytes("images/audio.mp3");
                base64String = Convert.ToBase64String(fileBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while converting the file to base64: " + ex.Message);
            }

            try
            {
                double balance = client.GetBalance();

                /* Upload a CAPTCHA and poll for its status. Put the audio base64 string,
                    the language, CAPTCHA type and desired solving timeout (in seconds)
                   here. If solved, you'll receive a DeathByCaptcha.Captcha object. */
                Captcha captcha = client.Decode(Client.DefaultTimeout,
                    new Hashtable()
                    {
                        {"type", 13},
                        {"language", "en"},
                        {"audio", base64String}
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
