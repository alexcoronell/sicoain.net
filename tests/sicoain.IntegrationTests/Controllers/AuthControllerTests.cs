using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;

namespace sicoain.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly CookieHandler _cookieHandler;

    public AuthControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _cookieHandler = new CookieHandler();
        _client = _factory.CreateDefaultClient(_cookieHandler);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new { Email = "notexists@test.com", Password = "WrongPass123!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        content.Should().NotBeNull();
        content!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndSetsCookies()
    {
        // Arrange
        await CreateTestUserAsync("test@test.com", "Test123!", "Admin");
        var loginRequest = new { Email = "test@test.com", Password = "Test123!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.Success.Should().BeTrue();
        authResponse.Email.Should().Be("test@test.com");

        var setCookies = response.Headers.GetValues("Set-Cookie").ToList();
        setCookies.Should().Contain(c => c.StartsWith("access_token="));
        setCookies.Should().Contain(c => c.StartsWith("refresh_token="));
    }

    [Fact]
    public async Task Refresh_WithValidRefreshToken_ReturnsNewTokens()
    {
        // Arrange: login first — CookieHandler stores cookies automatically
        await CreateTestUserAsync("refresh@test.com", "Refresh123!", "Admin");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login",
            new { Email = "refresh@test.com", Password = "Refresh123!" });
        loginResponse.EnsureSuccessStatusCode();

        // Act: CookieHandler sends the stored refresh_token cookie
        var refreshResponse = await _client.PostAsync("/api/v1/Auth/refresh", null);

        // Assert
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshCookies = refreshResponse.Headers.GetValues("Set-Cookie").ToList();
        refreshCookies.Should().Contain(c => c.StartsWith("access_token="));
    }

    [Fact]
    public async Task Logout_RevokesTokenAndClearsCookies()
    {
        // Arrange: login first
        await CreateTestUserAsync("logout@test.com", "Logout123!", "Admin");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login",
            new { Email = "logout@test.com", Password = "Logout123!" });
        loginResponse.EnsureSuccessStatusCode();

        // Act
        var logoutResponse = await _client.PostAsync("/api/v1/Auth/logout", null);

        // Assert
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var logoutCookies = logoutResponse.Headers.GetValues("Set-Cookie").ToList();
        logoutCookies.Should().Contain(c => c.Contains("access_token=;") && c.Contains("expires="));
        logoutCookies.Should().Contain(c => c.Contains("refresh_token=;") && c.Contains("expires="));
    }

    private async Task CreateTestUserAsync(string email, string password, string role)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null) return;

        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = "Test User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded && !string.IsNullOrEmpty(role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
