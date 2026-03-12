Imports System
Imports System.Threading
Imports System.Collections

Imports DeathByCaptcha

Public Class AmazonWaf
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

        ' Proxy and captcha data
        Dim proxy as String = "http://user:password@127.0.0.1:1234"
        Dim proxyType as String = "HTTP"
        Dim sitekey as String = "AQIDAHjcYu/GjX+QlghicBgQ/7bFaQZ+m5FKCMDnO+vTbNg96AHDh0IR5vgzHNceHYqZR+GOAAAAfjB8BgkqhkiG9w0BBwagbzBtAgEAMGgGCSqGSIb3DQEHATAeBglghkgBZQMEAS4wEQQMsYNQbVOLOfd/1ofjAgEQgDuhVKc2V/0XTEPc+9X/xAodxDqgyNNNyYJN1rM2gs4yBMeDXXc3z2ZxmD9jsQ8eNMGHqeii56iL2Guh4A=="
        Dim pageurl as String = "https://efw47fpad9.execute-api.us-east-1.amazonaws.com/latest"
        Dim iv as string = "CgAFRjIw2vAAABSM"
        Dim context as string = "zPT0jOl1rQlUNaldX6LUpn4D6Tl9bJ8VUQ/NrWFxPiiFujn5bFHzpOlKYQG0Di/UrO/p0xItkf7oGrknHqnj+UjvWv+i0BFbm3vGKceNaGtjrg4wvydL2Li5XjwRUOMW4o+NgO3JPJhkgwRKSyK62cIIzrThlOBD+gmtvKW0JNtH8efKR8Y5mBf0gi8JokjUxq/XbyB6h83tfaiWrp3dkOJsEXHLkT/wwQlFZysA919LCA+XVqgJ9lurUZqHWar+9JHqWnc0ghckKCnUzubvSQzJl+eSIAIoYZrpuZQszOwWzo4="

        Console.WriteLine(String.Format("Your balance is {0,2:f} US cents",
                                        client.Balance))

        ' Create a JSON with the extra data
        Dim wafParams as String = "{""proxy"": """ + proxy + """," +
                             """proxytype"": """ + proxyType + """," +
                             """sitekey"": """ + sitekey + """," +
                             """pageurl"": """ + pageurl + """," +
                             """iv"": """ + iv + """," +
                             """context"": """ + context + """}"

        ' Create the payload with the type and the extra data
        Dim extraData As New Hashtable()
        extraData.Add("type", 16)
        extraData.Add("waf_params", wafParams)

        ' Upload a CAPTCHA and poll for its status.  Put the Amazon Waf
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
