using System.Net.Http.Json;

namespace sicoain.IntegrationTests.Utilities;

public static class HttpClientExtensions
{
    public static async Task AddCsrfTokenAsync(this HttpClient client)
    {
        var response = await client.GetAsync("/swagger/index.html");
        var cookies = response.Headers.GetValues("Set-Cookie");
        var csrfCookie = cookies.FirstOrDefault(c => c.StartsWith("csrf_token="));
        if (csrfCookie != null)
        {
            var token = csrfCookie.Split(';')[0].Split('=')[1];
            client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);
        }
    }

    public static async Task<HttpResponseMessage> LoginAsync(this HttpClient client, string email, string password)
    {
        var loginRequest = new { Email = email, Password = password };
        var response = await client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        return response;
    }
}
