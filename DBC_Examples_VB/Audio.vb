Imports System
Imports System.IO
Imports System.Threading
Imports System.Collections

Imports DeathByCaptcha

Public Class Audio
    Public Shared Sub Main(ByVal args As String())

        ' Put your DBC username & password or authtoken here:
        Dim username as String = "your-username"
        Dim password as String = "your-password"
        Dim token_from_panel as String = "your-token-from-panel"

        ' DBC Socket API client
        ' Dim client As New SocketClient(username, password)
        ' DBC HTTP API client
        Dim client As New HttpClient(username, password)

        ' To use token auth the first parameter must be "authtoken"
        ' Dim client As New HttpClient("authtoken", token_from_panel) 

        ' Read the audio file and convert it to base64 string
        Dim base64String As String = Nothing
        Try
            Dim fileBytes As Byte() = File.ReadAllBytes("images/audio.mp3")
            base64String = Convert.ToBase64String(fileBytes)
        Catch ex As System.Exception
            Console.WriteLine("An error occurred while converting the file to base64: " & ex.Message)
        End Try

        Console.WriteLine(String.Format("Your balance is {0,2:f} US cents",
                                        client.Balance))


        ' Create the payload with the type and the data
        Dim extraData As New Hashtable()
        extraData.Add("type", 13)
        extraData.Add("language", "en")
        extraData.Add("audio", base64String)

        ' Upload a CAPTCHA and poll for its status. Put the Audio
        ' parameters, CAPTCHA type and desired solving timeout (in seconds)
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
