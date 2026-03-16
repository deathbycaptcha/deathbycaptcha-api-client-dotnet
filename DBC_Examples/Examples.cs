using System;
using System.Collections.Generic;
using System.Net;
using System.Collections;

using DeathByCaptcha;


namespace DeathByCaptcha
{
    public class ExampleSimple
    {
        static public void Main(string[] argv)
        {
            if(argv.Length < 2)
            {
                System.Console.WriteLine("Incorrect number of arguments");
                return;
            }
            // if using authentication token, username must be authtoken and password the authentication token from panel

            string dbcUsername = argv[0], dbcPassword = argv[1];

            // Put your DBC username & password here.
            //Client client = (Client)new HttpClient(dbcUsername, dbcPassword);
            Client client = (Client)new SocketClient(dbcUsername, dbcPassword);

            Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);

            for (int i = 2, l = argv.Length; i < l; i++) {
                Console.WriteLine("Solving CAPTCHA {0}", argv[i]);

                // Upload a CAPTCHA and poll for its status.  Put the CAPTCHA
                // image file name, file object, stream, or a vector of bytes,
                // and desired solving timeout (in seconds) here.  If solved,
                // you'll receive a DeathByCaptcha.Captcha object.
                Captcha captcha = client.Decode(argv[i], Client.DefaultTimeout);

				/*
				//Uploading captchas with type = 2 (Coordinates API)
				Captcha captcha = client.Decode(argv[i], Client.DefaultTimeout,
		         new Hashtable (){
		           { "type", 2 }
		        });

				//Uploading captchas with type = 3 (Image Group API)
				Captcha captcha = client.Decode(argv[i], Client.DefaultTimeout,
				  new Hashtable (){
					{ "type", 3 },
					{ "grid", "2x4" }, //optional grid parameter
					{"banner_text", "Select all images with meat"},
					{"banner", bannerFileName}
				});
				*/

				/*
				//Uploading captchas with type = 3 (Image Group API) and grid
				Captcha captcha = client.Decode(argv[i], Client.DefaultTimeout,
				  new Hashtable (){
					{ "type", 3 },
					{"banner_text", "Select all images with meat"},
					{"banner", bannerFileName},
					{"grid", "4x4"}
				});
				*/
				/*
				// Upload a CAPTCHA and poll for its status.  Put the Token CAPTCHA
				// Json payload, CAPTCHA type and desired solving timeout (in seconds)
				// here. If solved, you'll receive a DeathByCaptcha.Captcha object.
				Captcha captcha = client.Decode( Client.DefaultTokenTimeout,
					new Hashtable (){
						{ "type", 4 },
						{"token_params", "{\"proxy\": \"http://user:password@127.0.0.1:1234\",\"proxytype\": \"HTTP\",\"googlekey\": \"6Lc2fhwTAAAAAGatXTzFYfvlQMI2T7B6ji8UVV_f\",\"pageurl\": \"http://google.com\"}"}

					});
				*/
                if (null != captcha) {
                    Console.WriteLine("CAPTCHA {0:D} solved: {1}",
                                      captcha.Id, captcha.Text);

                    // Report an incorrectly solved CAPTCHA.  Make sure the
                    // CAPTCHA was in fact incorrectly solved, do not just
                    // report them all or at random, or you might be banned
                    // as abuser.
                    bool shouldReportIncorrect = false; // Set this after your own correctness validation.
                    if (shouldReportIncorrect) {
                        if (client.Report(captcha)) {
                            Console.WriteLine("Reported as incorrectly solved");
                        } else {
                            Console.WriteLine("Failed reporting as incorrectly solved");
                        }
                    }
                } else {
                    Console.WriteLine("CAPTCHA was not solved");
                }
            }

            Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);
        }
    }

    public class GetBalance
    {
        static public void Main(string[] argv)
        {
            if(argv.Length != 3)
            {
                System.Console.WriteLine("Incorrect number of arguments");
                return;
            }
            string dbcUsername = argv[0], dbcPassword = argv[1], clienttype = argv[2];
            Client client;
            if(clienttype == "HTTP"){
                System.Console.WriteLine("http client");
                client = (Client)new HttpClient(dbcUsername, dbcPassword);
            }else{
                System.Console.WriteLine("sockets client");
                client = (Client)new SocketClient(dbcUsername, dbcPassword);
            }
            Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);
        }
    }

	public class ExampleToken
	{
		static public void Main(string[] argv)
		{

            if (argv.Length < 2)
            {
                System.Console.WriteLine("Incorrect number of arguments");
                return;
            }
            // if using authentication token, username must be authtoken and password the authentication token from panel

            string dbcUsername = argv[0], dbcPassword = argv[1];

			// Put your DBC username & password here.
			//Client client = (Client)new HttpClient(dbcUsername, dbcPassword);
			Client client = (Client)new HttpClient(dbcUsername, dbcPassword);

			//Put your Proxy credentials and type here
			string proxy = "http://user:password@127.0.0.1:1234";
			string proxyType = "HTTP";
			Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);

			if (argv.Length==4){

				//Create the Json payload, Put the Site url and Sitekey here.
				string tokenParams = "{\"proxy\": \""+ proxy+"\"," +
					"\"proxytype\": \""+proxyType+"\"," +
					"\"googlekey\": \""+argv[3]+"\"," +
					"\"pageurl\": \""+argv[2]+"\"}";

				// Upload a CAPTCHA and poll for its status.  Put the Token CAPTCHA
				// Json payload, CAPTCHA type and desired solving timeout (in seconds)
				// here. If solved, you'll receive a DeathByCaptcha.Captcha object.
				Captcha captcha = client.Decode( Client.DefaultTimeout,
					new Hashtable (){
						{ "type", 4 },
						{"token_params", tokenParams}

					});

				if (null != captcha) {
					Console.WriteLine("CAPTCHA {0:D} solved: {1}",
						captcha.Id, captcha.Text);

					// Report an incorrectly solved CAPTCHA.  Make sure the
					// CAPTCHA was in fact incorrectly solved, do not just
					// report them all or at random, or you might be banned
					// as abuser.
                    bool shouldReportIncorrect = false; // Set this after your own correctness validation.
                    if (shouldReportIncorrect) {
						if (client.Report(captcha)) {
							Console.WriteLine("Reported as incorrectly solved");
						} else {
							Console.WriteLine("Failed reporting as incorrectly solved");
						}
					}
				} else {
					Console.WriteLine("CAPTCHA was not solved");
				}
			}

			Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);
		}
	}

    public class ExampleFull
    {
        static public Client client = null!;


        static public void Decode(object? o)
        {
            if (o is not string captchaFileName)
            {
                Console.WriteLine("Invalid CAPTCHA payload");
                return;
            }

            Console.WriteLine("Solving {0}", captchaFileName);

            // Put your CAPTCHA image file name, file object, stream or vector
            // of bytes here:
            Captcha captcha = ExampleFull.client.Upload(captchaFileName);
            if (null != captcha) {
                Console.WriteLine("CAPTCHA {0} uploaded: {1}",
                                  captchaFileName, captcha.Id);

                // Poll for the CAPTCHA status until it's solved.
                // Wait at least a few seconds between poll or you'll get
                // banned as abuser.
				int index = 0;
				int interval = 0;

				while (captcha.Uploaded && !captcha.Solved) {
					Client.GetPollInterval(index, out interval, out index);
					System.Threading.Thread.Sleep(interval * 1000);
                    captcha = ExampleFull.client.GetCaptcha(captcha.Id);
                }

                if (captcha.Solved) {
                    Console.WriteLine("CAPTCHA {0} solved: {1}",
                                      captchaFileName, captcha.Text);

                    // Report an incorrectly solved CAPTCHA.  Make sure the
                    // CAPTCHA was in fact incorrectly solved, do not just
                    // report them all or at random, or you might be banned
                    // as abuser.
                    bool shouldReportIncorrect = false; // Set this after your own correctness validation.
                    if (shouldReportIncorrect) {
                        if (ExampleFull.client.Report(captcha)) {
                            Console.WriteLine("CAPTCHA {0} reported as incorrectly solved",
                                              captchaFileName);
                        } else {
                            Console.WriteLine("Failed reporting as incorrectly solved");
                        }
                    }
                } else {
                    Console.WriteLine("CAPTCHA was not solved");
                }
            } else {
                Console.WriteLine("CAPTCHA was not uploaded");
            }
        }


        static public void Main(string[] argv)
        {

            if (argv.Length < 2)
            {
                System.Console.WriteLine("Incorrect number of arguments");
                return;
            }
            // if using authentication token, username must be authtoken and password the authentication token from panel

            string dbcUsername = argv[0], dbcPassword = argv[1];

            // Put your DBC username & password here.
            //ExampleFull.client = (Client)new HttpClient(dbcUsername, dbcPassword);
            ExampleFull.client = (Client)new HttpClient(dbcUsername, dbcPassword);

            Console.WriteLine("Your balance is {0:F2} US cents",
                              ExampleFull.client.Balance);

            List<System.Threading.Thread> threads =
                new List<System.Threading.Thread>();
            for (int i = 2, l = argv.Length; i < l; i++) {
                System.Threading.Thread t =
                    new System.Threading.Thread(ExampleFull.Decode);
                threads.Add(t);
                t.Start(argv[i]);
            }
            foreach (System.Threading.Thread t in threads) {
                t.Join();
            }

            Console.WriteLine("Your balance is {0:F2} US cents",
                              ExampleFull.client.Balance);
        }
    }


    public class ExampleAsync
    {
        static protected System.Threading.ManualResetEvent _ready =
            new System.Threading.ManualResetEvent(false);


        static protected void Decoded(Captcha captcha)
        {
            if (null != captcha) {
                Console.WriteLine("CAPTCHA {0:D} solved: {1}",
                                  captcha.Id, captcha.Text);
            } else {
                Console.WriteLine("CAPTCHA was not solved");
            }
            ExampleAsync._ready.Set();
        }


        static public void Main(string[] argv)
        {
            System.Console.WriteLine("ExampleAsync Runing!");

            if (argv.Length < 2)
            {
                System.Console.WriteLine("Incorrect number of arguments");
                return;
            }
            // if using authentication token, username must be authtoken and password the authentication token from panel

            string dbcUsername = argv[0], dbcPassword = argv[1];

            // Put your DBC username & password here.
            //Client client = (Client)new HttpClient(dbcUsername, dbcPassword);
            Client client = (Client)new HttpClient(dbcUsername, dbcPassword);

            Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);

            for (int i = 2, l = argv.Length; i < l; i++) {
                Console.WriteLine("Solving CAPTCHA {0}", argv[i]);

                // Upload a CAPTCHA and poll for its status.  Put the CAPTCHA
                // image file name, file object, stream, or a vector of bytes,
                // and desired solving timeout (in seconds) here:
                client.Decode(new DecodeDelegate(ExampleAsync.Decoded),
                              argv[i],
                              Client.DefaultTimeout);
                ExampleAsync._ready.WaitOne();
            }

            Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);
        }
    }
}
