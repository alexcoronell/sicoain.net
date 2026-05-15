using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs.Users;

namespace sicoain.api.Controllers
{
    public class UserController : BaseCrudController<UserDto, CreateUserRequest, UpdateUserRequest>
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetByEmailAsync([FromRoute] string email)
        {
            var user = await _userService.GetByEmailAsync(email).ConfigureAwait(false);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("email-exists/{email}")]
        [Authorize]
        public async Task<IActionResult> EmailExistsAsync([FromRoute] string email)
        {
            var exists = await _userService.EmailExistsAsync(email).ConfigureAwait(false);
            return Ok(new { exists });
        }

        [HttpPatch("assign-role/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoleAsync([FromRoute] int id, [FromBody] AssignOrRemoveRoleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userService.AssignRoleAsync(id, request.RoleName).ConfigureAwait(false);
            if (!result) return BadRequest(new { message = "Failed to assign role: User or role not found" });

            return Ok(new { message = "Role assigned successfully" });
        }

        [HttpPatch("remove-role/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRoleAsync([FromRoute] int id, [FromBody] AssignOrRemoveRoleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userService.RemoveRoleAsync(id, request.RoleName).ConfigureAwait(false);
            if (!result) return BadRequest(new { message = "Failed to remove role: User or role not found" });

            return Ok(new { message = "Role removed successfully" });
        }

        [HttpPatch("change-password/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromRoute] int id, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validaciones adicionales de negocio
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
            {
                return BadRequest(new { message = "All password fields are required" });
            }

            if (request.NewPassword != request.ConfirmNewPassword)
                return BadRequest(new { message = "New password and confirmation do not match" });

            var result = await _userService.ChangePasswordAsync(id, request).ConfigureAwait(false);
            if (!result) return BadRequest(new { message = "Password change failed. Check your current password." });

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("roles/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserRolesAsync([FromRoute] int id)
        {
            var result = await _userService.GetUserRolesAsync(id).ConfigureAwait(false);
            return Ok(result ?? Enumerable.Empty<string>());
        }
    }
}
