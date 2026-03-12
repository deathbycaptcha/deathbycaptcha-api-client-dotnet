Imports System
Imports System.Threading
Imports System.Collections

Imports DeathByCaptcha

Public Class Capy
    Public Shared Sub Main(ByVal args As String())

        ' Put your DBC username & password or authtoken here:
        Dim username = "username"
        Dim password = "password"
        Dim token_from_panel = "your-token-from-panel"

        ' DBC Socket API client
        ' Dim client As New SocketClient(username, password)
        ' DBC HTTP API client
        Dim client As New HttpClient(username, password)

        ' To use token auth the first parameter must be "authtoken"
        ' Dim client As New HttpClient("authtoken", token_from_panel) 

        Dim proxy as String = "http://user:password@127.0.0.1:1234"
        Dim proxyType as String = "HTTP"
        Dim captchakey as String = "6LdyC2cUAAAAACGuDKpXeDorzUDWXmdqeg-xy696"
        Dim pageurl as String = "https://www.capy.me/products/puzzle_captcha/"
        Dim api_server as String = "https://www.capy.me/"

        Console.WriteLine(String.Format("Your balance is {0,2:f} US cents",
                                        client.Balance))

        ' Create a JSON with the extra data
        Dim tokenParams as String = "{""proxy"": """ + proxy + """," +
                          """proxytype"": """ + proxyType + """," +
                          """captchakey"": """ + captchakey + """," +
                          """pageurl"": """ + pageurl + """," +
                          """api_server"": """ + api_server + """}"

        ' Create the payload with the type and the extra data
        Dim extraData As New Hashtable()
        extraData.Add("type", 15)
        extraData.Add("capy_params", tokenParams)

        ' Upload a CAPTCHA and poll for its status.  Put the Token CAPTCHA
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
