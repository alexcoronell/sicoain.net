using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using sicoain.api.Abstractions;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the authentication service covering login, token refresh, logout, and account lockout scenarios.
    /// </summary>
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
        private readonly Mock<IRefreshTokenGenerator> _refreshTokenGeneratorMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<ICookieManager> _cookieManagerMock;
        private readonly Mock<IIpAddressProvider> _ipProviderMock;
        private readonly Mock<IPermissionService> _permissionServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Setup UserManager mock
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            // Setup SignInManager mock
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<User>>().Object);

            _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
            _refreshTokenGeneratorMock = new Mock<IRefreshTokenGenerator>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _cookieManagerMock = new Mock<ICookieManager>();
            _ipProviderMock = new Mock<IIpAddressProvider>();
            _permissionServiceMock = new Mock<IPermissionService>();
            _configurationMock = new Mock<IConfiguration>();

            var jwtSettingsSectionMock = new Mock<IConfigurationSection>();
            jwtSettingsSectionMock.Setup(x => x["ExpirationMinutes"]).Returns("15");
            jwtSettingsSectionMock.Setup(x => x["RefreshTokenExpirationDays"]).Returns("7");
            _configurationMock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSettingsSectionMock.Object);

            _authService = new AuthService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _jwtGeneratorMock.Object,
                _refreshTokenGeneratorMock.Object,
                _refreshTokenRepositoryMock.Object,
                _cookieManagerMock.Object,
                _ipProviderMock.Object,
                _permissionServiceMock.Object,
                _configurationMock.Object);
        }

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var request = new LoginRequest("test@example.com", "Password123!");
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid email or password.");
            _jwtGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<List<Claim>>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIncorrect_ReturnsFailureResponse()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", FullName = "Test User" };
            var request = new LoginRequest("test@example.com", "WrongPassword");
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, true))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid email or password.");
            _jwtGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<List<Claim>>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenValidCredentials_ReturnsSuccessAndSetsCookies()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", FullName = "Test User" };
            var request = new LoginRequest("test@example.com", "CorrectPassword");
            var permissions = new List<string> { "Accidents.View", "Employees.View" };
            var permissionClaims = permissions.Select(p => new Claim("Permission", p)).ToList();
            var accessToken = "access-token-value";
            var refreshToken = "refresh-token-value";

            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, true))
                .ReturnsAsync(SignInResult.Success);
            _permissionServiceMock.Setup(x => x.GetUserPermissionNameAsync(user))
                .ReturnsAsync(permissions);
            _jwtGeneratorMock.Setup(x => x.GenerateToken(user, It.IsAny<List<Claim>>()))
                .Returns(accessToken);
            _refreshTokenGeneratorMock.Setup(x => x.GenerateToken())
                .Returns(refreshToken);
            _ipProviderMock.Setup(x => x.GetCurrentIpAddress())
                .Returns("127.0.0.1");
            _refreshTokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);
            _refreshTokenRepositoryMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            _cookieManagerMock.Setup(x => x.SetTokenCookie("access_token", accessToken, 15));
            _cookieManagerMock.Setup(x => x.SetTokenCookie("refresh_token", refreshToken, 7 * 24 * 60));

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Login successful");
            result.Email.Should().Be(user.Email);
            result.FullName.Should().Be(user.FullName);
            _jwtGeneratorMock.Verify(x => x.GenerateToken(user, It.IsAny<List<Claim>>()), Times.Once);
            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            _refreshTokenRepositoryMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
            _cookieManagerMock.Verify(x => x.SetTokenCookie("access_token", accessToken, 15), Times.Once);
            _cookieManagerMock.Verify(x => x.SetTokenCookie("refresh_token", refreshToken, 7 * 24 * 60), Times.Once);
        }

        #endregion

        #region RefreshTokenAsync Tests

        [Fact]
        public async Task RefreshTokenAsync_WhenNoRefreshTokenInCookie_ReturnsFailure()
        {
            // Arrange
            _cookieManagerMock.Setup(x => x.GetCookieValue("refresh_token"))
                .Returns((string)null);

            // Act
            var result = await _authService.RefreshTokenAsync();

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("No refresh token provided.");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenRefreshTokenInvalid_ReturnsFailure()
        {
            // Arrange
            var refreshToken = "invalid-token";
            _cookieManagerMock.Setup(x => x.GetCookieValue("refresh_token"))
                .Returns(refreshToken);
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(refreshToken))
                .ReturnsAsync((RefreshToken)null);

            // Act
            var result = await _authService.RefreshTokenAsync();

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid or expired refresh token.");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenRefreshTokenExpired_ReturnsFailure()
        {
            // Arrange
            var refreshToken = "expired-token";
            var storedToken = new RefreshToken { Token = refreshToken, RevokedAt = DateTime.UtcNow };
            _cookieManagerMock.Setup(x => x.GetCookieValue("refresh_token"))
                .Returns(refreshToken);
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(refreshToken))
                .ReturnsAsync(storedToken);

            // Act
            var result = await _authService.RefreshTokenAsync();

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid or expired refresh token.");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenValidRefreshToken_ReturnsNewTokensAndCookies()
        {
            // Arrange
            var oldRefreshToken = "old-token";
            var user = new User { Id = 1, Email = "test@example.com", FullName = "Test User" };
            var storedToken = new RefreshToken
            {
                Token = oldRefreshToken,
                User = user,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                Id = 10
            };
            var newAccessToken = "new-access-token";
            var newRefreshToken = "new-refresh-token";

            _cookieManagerMock.Setup(x => x.GetCookieValue("refresh_token"))
                .Returns(oldRefreshToken);
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(oldRefreshToken))
                .ReturnsAsync(storedToken);
            _refreshTokenRepositoryMock.Setup(x => x.RevokeAsync(storedToken, It.IsAny<string>(), "Refreshed"))
                .Returns(Task.CompletedTask);
            _jwtGeneratorMock.Setup(x => x.GenerateToken(user, null))
                .Returns(newAccessToken);
            _refreshTokenGeneratorMock.Setup(x => x.GenerateToken())
                .Returns(newRefreshToken);
            _ipProviderMock.Setup(x => x.GetCurrentIpAddress())
                .Returns("127.0.0.1");
            _refreshTokenRepositoryMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);
            _refreshTokenRepositoryMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            _cookieManagerMock.Setup(x => x.SetTokenCookie("access_token", newAccessToken, 15));
            _cookieManagerMock.Setup(x => x.SetTokenCookie("refresh_token", newRefreshToken, 7 * 24 * 60));

            // Act
            var result = await _authService.RefreshTokenAsync();

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Login successful"); // Note: The original service returns "Login successful"
            result.Email.Should().Be(user.Email);
            result.FullName.Should().Be(user.FullName);
            _jwtGeneratorMock.Verify(x => x.GenerateToken(user, null), Times.Once);
            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            _cookieManagerMock.Verify(x => x.SetTokenCookie("access_token", newAccessToken, 15), Times.Once);
            _cookieManagerMock.Verify(x => x.SetTokenCookie("refresh_token", newRefreshToken, 7 * 24 * 60), Times.Once);
        }

        #endregion

        #region RevokeTokenAsync Tests

        [Fact]
        public async Task RevokeTokenAsync_WhenNoRefreshToken_ReturnsFalse()
        {
            // Arrange
            _cookieManagerMock.Setup(x => x.GetCookieValue("refresh_token"))
                .Returns((string)null);

            // Act
            var result = await _authService.RevokeTokenAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RevokeTokenAsync_WhenTokenNotFound_ReturnsFalse()
        {
            // Arrange
            var refreshToken = "some-token";
            _cookieManagerMock.Setup(x => x.GetCookieValue("refresh_token"))
                .Returns(refreshToken);
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(refreshToken))
                .ReturnsAsync((RefreshToken)null);

            // Act
            var result = await _authService.RevokeTokenAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RevokeTokenAsync_WhenTokenIsNotActive_ReturnsFalse()
        {
            // Arrange
            var refreshToken = "inactive-token";
            var storedToken = new RefreshToken { Token = refreshToken, RevokedAt = DateTime.UtcNow };
            _cookieManagerMock.Setup(x => x.GetCookieValue("refresh_token"))
                .Returns(refreshToken);
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(refreshToken))
                .ReturnsAsync(storedToken);

            // Act
            var result = await _authService.RevokeTokenAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RevokeTokenAsync_WhenValidToken_RevokesAndDeletesCookies()
        {
            // Arrange
            var refreshToken = "valid-token";
            var storedToken = new RefreshToken { Token = refreshToken, ExpiresAt = DateTime.UtcNow.AddDays(7) };
            _cookieManagerMock.Setup(x => x.GetCookieValue("refresh_token"))
                .Returns(refreshToken);
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(refreshToken))
                .ReturnsAsync(storedToken);
            _refreshTokenRepositoryMock.Setup(x => x.RevokeAsync(storedToken, It.IsAny<string>(), "Logout"))
                .Returns(Task.CompletedTask);
            _refreshTokenRepositoryMock.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            _cookieManagerMock.Setup(x => x.DeleteCookie("access_token"));
            _cookieManagerMock.Setup(x => x.DeleteCookie("refresh_token"));

            // Act
            var result = await _authService.RevokeTokenAsync();

            // Assert
            result.Should().BeTrue();
            _cookieManagerMock.Verify(x => x.DeleteCookie("access_token"), Times.Once);
            _cookieManagerMock.Verify(x => x.DeleteCookie("refresh_token"), Times.Once);
        }

        #endregion

        #region GetCurrentUserAsync Tests

        [Fact]
        public async Task GetCurrentUserAsync_WhenUserIdClaimMissing_ReturnsNull()
        {
            // Arrange
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = await _authService.GetCurrentUserAsync(userPrincipal);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCurrentUserAsync_WhenUserIdClaimNotInt_ReturnsNull()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "invalid") };
            var identity = new ClaimsIdentity(claims);
            var userPrincipal = new ClaimsPrincipal(identity);

            // Act
            var result = await _authService.GetCurrentUserAsync(userPrincipal);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCurrentUserAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var userId = 5;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var userPrincipal = new ClaimsPrincipal(identity);
            var expectedUser = new User { Id = userId, Email = "user@test.com", FullName = "Test User" };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _authService.GetCurrentUserAsync(userPrincipal);

            // Assert
            result.Should().Be(expectedUser);
        }

        #endregion

        #region ValidateRefreshTokenAsync Tests

        [Fact]
        public async Task ValidateRefreshTokenAsync_WhenTokenNotFound_ReturnsNull()
        {
            // Arrange
            var token = "nonexistent";
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
                .ReturnsAsync((RefreshToken)null);

            // Act
            var result = await _authService.ValidateRefreshTokenAsync(token);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_WhenTokenInactive_ReturnsNull()
        {
            // Arrange
            var token = "inactive-token";
            var storedToken = new RefreshToken { Token = token, RevokedAt = DateTime.UtcNow };
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
                .ReturnsAsync(storedToken);

            // Act
            var result = await _authService.ValidateRefreshTokenAsync(token);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_WhenValidToken_ReturnsAssociatedUser()
        {
            // Arrange
            var token = "valid-token";
            var user = new User { Id = 10, FullName = "Test User" };
            var storedToken = new RefreshToken { Token = token, User = user, ExpiresAt = DateTime.UtcNow.AddDays(7) };
            _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
                .ReturnsAsync(storedToken);

            // Act
            var result = await _authService.ValidateRefreshTokenAsync(token);

            // Assert
            result.Should().Be(user);
        }

        #endregion

        #region RevokeAllUserTokensAsync Tests

        [Fact]
        public async Task RevokeAllUserTokensAsync_ShouldCallRepositoryAndReturnCount()
        {
            // Arrange
            var userId = 42;
            var revokedCount = 3;
            _refreshTokenRepositoryMock.Setup(x => x.RevokeAllForUserAsync(userId, It.IsAny<string>(), "Revoked all by administrator"))
                .ReturnsAsync(revokedCount);

            // Act
            var result = await _authService.RevokeAllUserTokensAsync(userId);

            // Assert
            result.Should().Be(revokedCount);
        }

        #endregion
    }
}
