Imports System
Imports System.Threading
Imports System.Collections

Imports DeathByCaptcha

Public Class RecaptchaV3
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

        ' Proxy and recaptcha_v3 token data
        ' - recaptcha_v3 requires 'action' that is the action that triggers
        ' recaptcha_v3 validation
        ' - if 'action' isn't provided we use the default value "verify"
        ' - also you need to provide 'min_score', a number from 0.1 to 0.9,
        ' this is the minimum score acceptable from recaptchaV3
        Dim proxy as String = "http://user:password@127.0.0.1:1234"
        Dim proxyType as String = "HTTP"
        Dim googlekey as String = "6LdyC2cUAAAAACGuDKpXeDorzUDWXmdqeg-xy696"
        Dim pageurl as String = "https://recaptcha-demo.appspot.com/recaptcha-v3-request-scores.php"
        Dim action as String = "examples/v3scores"
        Dim min_score as String = "0.3"

        Console.WriteLine(String.Format("Your balance is {0,2:f} US cents",
                                        client.Balance))

        ' Create a JSON with the extra data
        Dim tokenParams as String = "{""proxy"": """ + proxy + """," +
                          """proxytype"": """ + proxyType + """," +
                          """googlekey"": """ + googlekey + """," +
                          """pageurl"": """ + pageurl + """," +
                          """action"": """ + action + """," +
                          """min_score"": """ + min_score + """}"

        ' Create the payload with the type and the extra data
        Dim extraData As New Hashtable()
        extraData.Add("type", 5)
        extraData.Add("token_params", tokenParams)

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
