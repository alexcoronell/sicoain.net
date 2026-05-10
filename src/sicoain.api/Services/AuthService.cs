using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.Entities;

namespace sicoain.api.Services;

/// <summary>
/// Service for authentication, token management, and user session handling.
/// </summary>
internal class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICookieManager _cookieManager;
    private readonly IIpAddressProvider _ipProvider;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        ICookieManager cookieManager,
        IIpAddressProvider ipProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _cookieManager = cookieManager;
        _ipProvider = ipProvider;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
        if (user == null)
        {
            return new AuthResponse { Success = false, Message = "Invalid email or password." };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return new AuthResponse { Success = false, Message = "Invalid email or password." };
        }

        var accessToken = _jwtGenerator.GenerateToken(user);
        var refreshToken = _refreshTokenGenerator.GenerateToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = _ipProvider.GetCurrentIpAddress(),
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(refreshTokenEntity).ConfigureAwait(false);
        await _refreshTokenRepository.SaveChangesAsync().ConfigureAwait(false);

        _cookieManager.SetTokenCookie("access_token", accessToken, 15);
        _cookieManager.SetTokenCookie("refresh_token", refreshToken, 7 * 24 * 60);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Email = user.Email,
            FullName = user.FullName,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync()
    {
        var refreshToken = _cookieManager.GetCookieValue("refresh_token");
        if (string.IsNullOrEmpty(refreshToken))
        {
            return new AuthResponse { Success = false, Message = "No refresh token provided." };
        }

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken).ConfigureAwait(false);
        if (storedToken == null || !storedToken.IsActive)
        {
            return new AuthResponse { Success = false, Message = "Invalid or expired refresh token." };
        }

        await _refreshTokenRepository.RevokeAsync(storedToken, _ipProvider.GetCurrentIpAddress(), "Refreshed").ConfigureAwait(false);

        var user = storedToken.User;
        var newAccessToken = _jwtGenerator.GenerateToken(user);
        var newRefreshToken = _refreshTokenGenerator.GenerateToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = _ipProvider.GetCurrentIpAddress(),
            CreatedAt = DateTime.UtcNow,
            ReplacedByTokenId = storedToken.Id
        };
        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity).ConfigureAwait(false);
        await _refreshTokenRepository.SaveChangesAsync().ConfigureAwait(false);

        _cookieManager.SetTokenCookie("access_token", newAccessToken, 15);
        _cookieManager.SetTokenCookie("refresh_token", newRefreshToken, 7 * 24 * 60);

        return new AuthResponse
        {
            Success = true,
            Message = "Token refreshed",
            Email = user.Email,
            FullName = user.FullName,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<bool> RevokeTokenAsync()
    {
        var refreshToken = _cookieManager.GetCookieValue("refresh_token");
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken).ConfigureAwait(false);
        if (storedToken == null || !storedToken.IsActive)
            return false;

        await _refreshTokenRepository.RevokeAsync(storedToken, _ipProvider.GetCurrentIpAddress(), "Logout").ConfigureAwait(false);
        await _refreshTokenRepository.SaveChangesAsync().ConfigureAwait(false);

        _cookieManager.DeleteCookie("access_token");
        _cookieManager.DeleteCookie("refresh_token");

        return true;
    }

    public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
    {
        var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return null;

        return await _userManager.FindByIdAsync(userId.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
    }

    public async Task<User?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken).ConfigureAwait(false);
        if (storedToken == null || !storedToken.IsActive)
            return null;

        return storedToken.User;
    }

    public async Task<int> RevokeAllUserTokensAsync(int userId)
    {
        return await _refreshTokenRepository.RevokeAllForUserAsync(userId, _ipProvider.GetCurrentIpAddress(), "Revoked all by administrator").ConfigureAwait(false);
    }
}
