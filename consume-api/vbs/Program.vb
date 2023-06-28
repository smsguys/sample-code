Imports System
Imports System.Threading.Tasks

Module Program
    Sub Main()
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Async Function MainAsync() As Task
        Dim keyCloakService = New KeyCloakService()
        Dim accessToken = Await keyCloakService.ObtainAccessTokenAsync()

        Dim smsService = New SmsService(accessToken)
        Dim success = Await smsService.SendSmsAsync("Hello, this is a test SMS.")

        If success Then
            Console.WriteLine("SMS sent successfully!")
        Else
            Console.WriteLine("Failed to send SMS.")
        End If
    End Function
End Module
