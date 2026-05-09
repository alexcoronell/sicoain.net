using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.Entities;
using System.Security.Cryptography;

namespace sicoain.api.Services;

internal class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("fullName", user.FullName)
        };

        var expirationMinutes = double.Parse(jwtSettings["ExpirationMinutes"]!, CultureInfo.InvariantCulture);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private void SetTokenCookie(string key, string token, int minutes)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_httpContextAccessor.HttpContext!.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(minutes, CultureInfo.InvariantCulture))
        };
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, token, cookieOptions);
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

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = GetIpAddress(),
            CreatedAt = DateTime.UtcNow
        };
        await _context.RefreshTokens.AddAsync(refreshTokenEntity).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        SetTokenCookie("access_token", accessToken, 15);
        SetTokenCookie("refresh_token", refreshToken, 7 * 24 * 60);

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
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return new AuthResponse { Success = false, Message = "No refresh token provided." };
        }

        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken).ConfigureAwait(false);

        if (storedToken == null || !storedToken.IsActive)
        {
            return new AuthResponse { Success = false, Message = "Invalid or expired refresh token." };
        }

        storedToken.Revoke(GetIpAddress(), "Refreshed");

        var user = storedToken.User;
        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = GetIpAddress(),
            CreatedAt = DateTime.UtcNow,
            ReplacedByTokenId = storedToken.Id
        };
        await _context.RefreshTokens.AddAsync(newRefreshTokenEntity).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        SetTokenCookie("access_token", newAccessToken, 15);
        SetTokenCookie("refresh_token", newRefreshToken, 7 * 24 * 60);

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
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken).ConfigureAwait(false);
        if (storedToken == null || !storedToken.IsActive)
            return false;

        storedToken.Revoke(GetIpAddress(), "Logout");
        await _context.SaveChangesAsync().ConfigureAwait(false);

        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("access_token");
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh_token");

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
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken).ConfigureAwait(false);

        if (storedToken == null || !storedToken.IsActive)
            return null;

        return storedToken.User;
    }

    public async Task<int> RevokeAllUserTokensAsync(int userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync().ConfigureAwait(false);

        foreach (var token in tokens)
            token.Revoke(GetIpAddress(), "Revoked all by administrator");

        await _context.SaveChangesAsync().ConfigureAwait(false);
        return tokens.Count;
    }
}
