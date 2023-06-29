# Introduction

This Visual Basic .NET code describes a simplified sample console application that sends an SMS message using the `KeyCloakService` and `SmsService` classes.

**Imports**

```vbnet
Imports System
Imports System.Threading.Tasks
```

These are the namespaces imported into the code file:

- `System` is used for base data types and core functionality.
- `System.Threading.Tasks` is used for asynchronous programming.

**Program Module**

```vbnet
Module Program
```

The `Program` module is the entry point for this console application.

```vbnet
    Sub Main()
        MainAsync().GetAwaiter().GetResult()
    End Sub
```

The `Main` subroutine is the entry point of the console application. It calls the `MainAsync` function, which is an asynchronous method, and waits for it to complete by calling `GetAwaiter().GetResult()`. This allows the `Main` subroutine to run the `MainAsync` function synchronously.

```vbnet
    Async Function MainAsync() As Task
        Dim keyCloakService = New KeyCloakService()
        Dim accessToken = Await keyCloakService.ObtainAccessTokenAsync()
```

In the `MainAsync` function, an instance of `KeyCloakService` is created and used to obtain an access token asynchronously.

```vbnet
        Dim smsService = New SmsService(accessToken)
        Dim success = Await smsService.SendSmsAsync("Hello, this is a test SMS.")
```

The `MainAsync` function then creates an instance of `SmsService`, passing in the obtained access token. The `SendSmsAsync` method is then used to asynchronously send an SMS message.

```vbnet
        If success Then
            Console.WriteLine("SMS sent successfully!")
        Else
            Console.WriteLine("Failed to send SMS.")
        End If
    End Function
End Module
```

The `MainAsync` function concludes by checking the boolean `success` variable returned by the `SendSmsAsync` method. If `True`, it prints "SMS sent successfully!" to the console. If `False`, it prints "Failed to send SMS.". The `MainAsync` function then ends, returning the completed task to the `Main` subroutine.

It is important to note that both `KeyCloakService` and `SmsService` are not included in this piece of code and need to be provided for this code to run successfully.

# Sending an SMS

This code defines an `SmsService` class that communicates with an SMS API (https://api.smsguys.co.za/) to send SMS messages.

**Imports**

```vbnet
Imports System.Net.Http
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Net.Http.Headers
Imports System.Text
```

These namespaces are imported into the code file:

- `System.Net.Http` is used for HTTP client communication.
- `System.Text.Json` and `System.Text.Json.Serialization` are used for JSON serialization and deserialization.
- `System.Net.Http.Headers` is used to handle HTTP headers.
- `System.Text` is used for working with strings, specifically for character encoding.

**SmsService Class**

This class encapsulates the functionality of interacting with the SMS API.

```vbnet
Public Class SmsService
    Private httpClient As HttpClient
```

The `httpClient` is a private instance variable that will be used to make HTTP requests.

```vbnet
    Public Sub New(token As String)
        httpClient = New HttpClient()
        httpClient.BaseAddress = New Uri("https://api.smsguys.co.za/")
        httpClient.DefaultRequestHeaders.Accept.Clear()
        httpClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
    End Sub
```

The class constructor accepts an access token as a parameter and initializes the HTTP client. The client's base address is set to the API URL, its default Accept header is set to "application/json", and its Authorization header is set to use Bearer authentication with the provided token.

```vbnet
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
```

The `SendSmsAsync` method accepts a string representing the content of the SMS to be sent. It creates a new anonymous object `message` with several properties. This object is structured according to the API's expected request format.

```vbnet
        Dim jsonContent = New StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json")
        Dim response = Await httpClient.PostAsync("api/v1/sms/create", jsonContent)
        If response.IsSuccessStatusCode Then
            Console.WriteLine("SMS sent successfully. Status code: " & response.StatusCode)
            Dim responseContent = Await response.Content.ReadAsStringAsync()
            Console.WriteLine("Response JSON: " & responseContent)
            Return True
        Else
            Console.WriteLine("Failed to send SMS. Status code: " & response.StatusCode)
            Return False
        End If
    End Function
End Class
```

This portion of the method serializes the `message` object to a JSON string and sends it as the body of a POST request to the API endpoint for creating SMS messages. If the response indicates success (HTTP 2xx status), the method prints a success message to the console along with the HTTP status code and the response content, then returns `True`. If the response indicates a failure, the method prints a failure message and the HTTP status code, then returns `False`.

## Keycloak

The provided code is for an authentication mechanism using Keycloak, an open-source software product to allow single sign-on with Identity and Access Management aimed at modern applications and services.

**Imports**

```vbnet
Imports System.Net.Http
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Threading.Tasks
Imports System.Collections.Generic
```

These are the namespaces imported into the code file:

- `System.Net.Http` is used for HTTP client communication.
- `System.Text.Json` and `System.Text.Json.Serialization` are used for JSON serialization and deserialization.
- `System.Threading.Tasks` is used for asynchronous programming.
- `System.Collections.Generic` is used for working with collections of objects.

**TokenResponse Class**

This class is used to map the JSON response of the token request to an object.

```vbnet
Public Class TokenResponse
    <JsonPropertyName("access_token")>
    Public Property AccessToken As String
    <JsonPropertyName("expires_in")>
    Public Property ExpiresIn As Integer
    <JsonPropertyName("refresh_expires_in")>
    Public Property RefreshExpiresIn As Integer
    <JsonPropertyName("refresh_token")>
    Public Property RefreshToken As String
    <JsonPropertyName("token_type")>
    Public Property TokenType As String
    <JsonPropertyName("not-before-policy")>
    Public Property NotBeforePolicy As Integer
    <JsonPropertyName("session_state")>
    Public Property SessionState As String
    <JsonPropertyName("scope")>
    Public Property Scope As String
End Class
```

Each property corresponds to a specific part of the token response:

- `AccessToken` is the token to access the Keycloak service.
- `ExpiresIn` is the time in seconds until the access token expires.
- `RefreshExpiresIn` is the time in seconds until the refresh token expires.
- `RefreshToken` is the token to renew the AccessToken.
- `TokenType` is the type of the token, usually "Bearer".
- `NotBeforePolicy` is the timestamp indicating when the token will become valid.
- `SessionState` is the session state identifier.
- `Scope` is the scope of the access token.

**KeyCloakService Class**

This class handles the process of obtaining an access token from the Keycloak server.

```vbnet
Public Class KeyCloakService
    Private Const keycloakUrl As String = "https://ker.smsguys.co.za/realms/quarkus/protocol/openid-connect/token"
    Private Const clientId As String = "backend-service"
    Private Const clientSecret As String = "secret"
    Private Const username As String = "alice"
    Private Const password As String = "alice"

    Public Async Function ObtainAccessTokenAsync() As Task(Of String)
        Dim client = New HttpClient()

        Dim requestBody = New Dictionary(Of String, String) From {
            {"username", username},
            {"password", password},
            {"grant_type", "password"},
            {"client_id", clientId},
            {"client_secret", clientSecret}
        }

        Dim response = Await client.PostAsync(keycloakUrl, New FormUrlEncodedContent(requestBody))

        If response.IsSuccessStatusCode Then
            Dim jsonResponse = Await response.Content.ReadAsStringAsync()
            Dim tokenResponse = JsonSerializer.Deserialize(Of TokenResponse)(jsonResponse)
            Return tokenResponse.AccessToken
        Else
            Throw New Exception("Failed to obtain access token. Status code: " & response.StatusCode)
        End If
    End Function
End Class
```

The `ObtainAccessTokenAsync` method:

- Creates an HTTP client
- Prepares the request body
- Makes a POST request to the Keycloak server with the required parameters
- If the

 response status code indicates success (HTTP 2xx), it reads the response content as a string and deserializes the JSON response into the `TokenResponse` object.
- If the response is successful, it returns the access token.
- If the response status code indicates a failure, it throws an exception.
