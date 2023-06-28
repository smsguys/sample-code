Imports System.Net.Http
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Threading.Tasks
Imports System.Collections.Generic

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
