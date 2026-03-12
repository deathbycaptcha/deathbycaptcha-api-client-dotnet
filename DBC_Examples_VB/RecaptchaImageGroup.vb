Imports System
Imports System.Threading
Imports System.Collections

Imports DeathByCaptcha

Public Class RecaptchaImageGroup
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
        
        Dim filename = "./images/test2.jpg"
        Dim banner = "./images/banner.jpg"
        Dim banner_text = "choose all pizza:"
        
        Console.WriteLine(String.Format("Your balance is {0,2:f} US cents",
                                        client.Balance))
        
        ' Create the extra data with type
        Dim extraData as new Hashtable()
        extraData.Add("type", 3)
        extraData.Add("banner", banner)
        extraData.Add("banner_text", banner_text)
        ' If you want to specify grid data: extraData.Add("grid", "4x4")
        ' If you wont supply  grid parameter, dbc would attempt to autodetect proper grid.
        
        ' Upload a CAPTCHA and poll for its status.  Put the Token CAPTCHA
        ' Json payload, CAPTCHA type and desired solving timeout (in seconds)  
        ' here. If solved, you'll receive a DeathByCaptcha.Captcha object.
        Dim captcha As Captcha = client.Decode(filename, DeathByCaptcha.Client.DefaultTimeout, extraData)
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
