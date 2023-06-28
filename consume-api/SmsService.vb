Imports System.Net.Http
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Net.Http.Headers
Imports System.Text


Public Class SmsService
    Private httpClient As HttpClient

    Public Sub New(token As String)

        httpClient = New HttpClient()
        httpClient.BaseAddress = New Uri("https://api.smsguys.co.za/")
        httpClient.DefaultRequestHeaders.Accept.Clear()
        httpClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        'Add the access token obtained to the http headers
        httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
    End Sub

    Public Async Function SendSmsAsync(smsContent As String) As Task(Of Boolean)
        Dim message = New With {
                .requestId = Guid.NewGuid().ToString(),
                .id = Guid.NewGuid().ToString(),
                .messages = New List(Of Object) From {
                    New With {
                        .messageId = Guid.NewGuid().ToString(),
                        .content = smsContent,
                        .phoneNumber = "string", ' Replace with actual phone number
                        .cutOffTime = 0
                    }
                }
            }

        Dim jsonContent = New StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json")
        Dim response = Await httpClient.PostAsync("api/v1/sms/create", jsonContent)
        If response.IsSuccessStatusCode Then
            ' Handle success
            Console.WriteLine("SMS sent successfully. Status code: " & response.StatusCode)
            ' Read the response content And display it.
            Dim responseContent = Await response.Content.ReadAsStringAsync()
            Console.WriteLine("Response JSON: " & responseContent)
            Return True
        Else
            ' Handle failure
            Console.WriteLine("Failed to send SMS. Status code: " & response.StatusCode)
            Return False
        End If
    End Function
End Class
