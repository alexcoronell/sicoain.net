using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;

namespace sicoain.api.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _authService.LoginAsync(request).ConfigureAwait(false);

            if (!response.Success) return Unauthorized(response);

            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refrash()
        {
            var response = await _authService.RefreshTokenAsync().ConfigureAwait(false);

            if (!response.Success) return Unauthorized(response);

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.RevokeTokenAsync().ConfigureAwait(false);

            if (!result) return BadRequest(new { message = "Logout failed" });

            return Ok(new { message = "Logout successful" });
        }

        [HttpPost("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _authService.GetCurrentUserAsync(User).ConfigureAwait(false);

            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName
            });
        }
    }
}
