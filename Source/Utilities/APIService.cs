using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Entities;
using Utilities.Tokens;

// Service class for handling API requests and authentication.
public class ApiService
{
    private TokenStorage _tokenStorage; // Storage for JWT tokens.
    private readonly HttpClient _client = new HttpClient(); // HTTP client for making requests.
    private readonly string _apiBaseUrl = "https://timetower-server.onrender.com";  // Base URL of the API.
    private string userAuthToken;  // Stores the JWT token.
    private string _callbackUrl; // URL for the callback after authentication.
    private string _clientRequestId; // Unique identifier for client requests.
    private bool _isRedirecting = false; // Controls whether a redirect is happening.

    // Properties to check status and manage callback URLs.
    public bool IsCallbackReceived => !string.IsNullOrEmpty(_callbackUrl);
    public string CallbackUrl => _callbackUrl;
    public bool IsRedirecting => _isRedirecting;

    // Constructor initializes token storage and loads the stored token.
    public ApiService()
    {
        _tokenStorage = new TokenStorage();
        userAuthToken = _tokenStorage.LoadToken(); // Load stored token on initialization.
    }

    // Generates a unique client request identifier.
    private string GenerateClientRequestId()
    {
        return Guid.NewGuid().ToString();
    }

    // Redirects the user to the login page.
    public void RedirectToLogin()
    {
        if (_isRedirecting) return; // Prevent multiple redirects.

        _isRedirecting = true; // Mark as redirecting.
        try
        {
            _clientRequestId = GenerateClientRequestId(); // Generate a unique request ID.
            string url = $"https://timetower-web.vercel.app/register?clientRequestId={_clientRequestId}";

            // Start the default browser with the specified URL.
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true // Open in the default browser.
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while trying to open the browser: {ex.Message}");
        }
        finally
        {
            _isRedirecting = false; // Mark as not redirecting, even if there was an error.
        }
    }

    // Checks if the user is authenticated by making a request to the API.
    public async Task<string> IsAuth()
    {
        try
        {
            // Send GET request to verify client request ID.
            var response = await _client.GetAsync($"{_apiBaseUrl}/api/user/{_clientRequestId}/verify").ConfigureAwait(false);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during the verification of clientRequestId: {ex.Message}");
            return null; // Return null on failure.
        }
    }

    // Checks the authentication status in a loop until a callback is received.
    public async Task CheckAuthStatus()
    {
        if (_isRedirecting) return; // Ensure we are not redirecting.

        string callbackUrl = null;

        // Start polling for authentication.
        while (string.IsNullOrEmpty(callbackUrl))
        {
            callbackUrl = await IsAuth(); // Check authentication.

            if (!string.IsNullOrEmpty(callbackUrl))
            {
                HandleCallback(callbackUrl); // Handle the callback.

                if (await GetUserProfile() != null)
                {
                    Console.WriteLine("Logged in");
                    break; // Exit loop when profile is received.
                }
                else
                {
                    Console.WriteLine("Failed to get user profile.");
                    StoreToken(null); // Store null token on failure.
                    break; // Exit loop if failure occurs.
                }
            }
            else
            {
                Console.WriteLine("Still waiting for authentication...");
                await Task.Delay(2000).ConfigureAwait(false); // Wait 2 seconds before checking again.
            }
        }
    }

    // Handles the callback from the authentication process.
    public void HandleCallback(string callbackUrl)
    {
        string token = callbackUrl; // Extract token from callback URL.

        if (!string.IsNullOrEmpty(token))
        {
            StoreToken(token); // Store the token if valid.
        }
        else
        {
            Console.WriteLine("Failed to retrieve authentication token.");
        }
    }

    // Stores the JWT token in memory and persistent storage.
    private void StoreToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            userAuthToken = null; // Clear stored token.
            _tokenStorage.SaveToken(null); // Persist null token.
        }
        else
        {
            var json = JsonDocument.Parse(token); // Parse the token JSON.
            userAuthToken = json.RootElement.GetProperty("_token").ToString(); // Extract token.
            _tokenStorage.SaveToken(userAuthToken); // Save token in storage.
        }
    }

    // Fetches the user profile using the stored JWT token.
    public async Task<User> GetUserProfile()
    {
        var response = await _client.GetAsync($"{_apiBaseUrl}/api/user/{userAuthToken}/profile").ConfigureAwait(false);
        string json = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null; // Handle response.

        if (json == null)
        {
            StoreToken(null); // Store null token on failure.
            return null; // Return null on failure.
        }

        var jsonDoc = JsonDocument.Parse(json); // Parse the user profile JSON.

        return new User(
            jsonDoc.RootElement.GetProperty("id").GetInt32(),
            jsonDoc.RootElement.GetProperty("username").ToString(),
            jsonDoc.RootElement.GetProperty("highScore").GetInt32()
        ); // Create and return User object.
    }

    // Updates the user's high score in the API.
    public async Task UpdateHighScore(int highScore)
    {
        var jsonContent = JsonContent.Create(new { highScore }); // Create JSON content for the request.

        // Send PATCH request to update the high score.
        var response = await _client.PatchAsync($"{_apiBaseUrl}/api/user/{userAuthToken}/highscore", jsonContent).ConfigureAwait(false);

        // Handle the response.
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.Content.ToString()); // Log success.
        }
        else
        {
            Console.WriteLine("Failed to update the high score"); // Handle failure.
        }
    }
}
