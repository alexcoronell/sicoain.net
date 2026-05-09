using System.Security.Claims;
using sicoain.shared.DTOs;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    internal interface IAuthService
    {
        /// <summary>
        /// Authenticates a user and generates access/refresh tokens as HttpOnly cookies
        /// </summary>
        /// <param name="request">Login credentials (email and password)</param>
        /// <returns>Authentication response with user data (tokens are set as cookies)</returns>
        Task<AuthResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token from cookie
        /// </summary>
        /// <returns>New authentication response with fresh tokens set as cookies</returns>
        Task<AuthResponse> RefreshTokenAsync();

        /// <summary>
        /// Revokes the refresh token (logout) and clears authentication cookies
        /// </summary>
        /// <returns>True if revocation was successful, false otherwise</returns>
        Task<bool> RevokeTokenAsync();

        /// <summary>
        /// Gets the currently authenticated user from the HttpContext
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the current user</param>
        /// <returns>User object if authenticated, null otherwise</returns>
        Task<User?> GetCurrentUserAsync(ClaimsPrincipal user);

        /// <summary>
        /// Validates a refresh token and returns the associated user if valid
        /// </summary>
        /// <param name="refreshToken">The refresh token string from cookie</param>
        /// <returns>The user associated with the valid token, null otherwise</returns>
        Task<User?> ValidateRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revokes all refresh tokens for a specific user (used when password changes or suspected breach)
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>Number of tokens revoked</returns>
        Task<int> RevokeAllUserTokensAsync(int userId);
    }
}
