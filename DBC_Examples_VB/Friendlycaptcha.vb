Imports System
Imports System.Threading
Imports System.Collections

Imports DeathByCaptcha

Public Class Friendlycaptcha
    Public Shared Sub Main(ByVal args As String())

        ' Put your DBC username & password or authtoken here:
        Dim username as String = "username"
        Dim password as String = "password"
        Dim token_from_panel as String = "your-token-from-panel"

        ' DBC Socket API client
        ' Dim client As New SocketClient(username, password)
        ' DBC HTTP API client
        Dim client As New HttpClient(username, password)

        ' To use token auth the first parameter must be "authtoken"
        ' Dim client As New HttpClient("authtoken", token_from_panel) 

        ' Proxy and friendly captcha data
        Dim proxy as String = "http://user:password@127.0.0.1:1234"
        Dim proxyType as String = "HTTP"
        Dim sitekey as String = "FCMGEMUD2KTDSQ5H"
        Dim pageurl as String = "https://friendlycaptcha.com/demo"

        Console.WriteLine(String.Format("Your balance is {0,2:f} US cents",
                                        client.Balance))

        ' Create a JSON with the extra data
        Dim friendlycaptchaParams as String = "{""proxy"": """ + proxy + """," +
                             """proxytype"": """ + proxyType + """," +
                             """sitekey"": """ + sitekey + """," +
                             """pageurl"": """ + pageurl + """}"

        ' Create the payload with the type and the extra data
        Dim extraData As New Hashtable()
        extraData.Add("type", 20)
        extraData.Add("friendly_params", friendlycaptchaParams)

        ' Upload a CAPTCHA and poll for its status.  Put the Friendly Captcha
        ' Json payload, CAPTCHA type and desired solving timeout (in seconds)
        ' here. If solved, you'll receive a DeathByCaptcha.Captcha object.
        Dim captcha As Captcha = client.Decode(DeathByCaptcha.Client.DefaultTimeout, extraData)
        If captcha IsNot Nothing Then
            Console.WriteLine(String.Format("CAPTCHA {0:d} solved: {1}", captcha.Id,
                                            captcha.Text))

            ' Report an incorrectly solved CAPTCHA.
            ' Make sure the CAPTCHA was in fact incorrectly solved, do not
            ' just report it at random, or you might be banned as abuser.
            ' If client.Report(captcha) Then
            '    Console.WriteLine("Reported as incorrectly solved")
            ' Else
            '    Console.WriteLine("Failed reporting as incorrectly solved")
            ' End If
        End If
    End Sub
End Class
