using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.api.Exceptions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Permissions;
using sicoain.shared.DTOs.Roles;

namespace sicoain.api.Controllers
{
    public class RolesController : BaseCrudController<RoleDto, CreateRoleRequest, UpdateRoleRequest>
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public RolesController(IRoleService roleService, IPermissionService permissionService) : base(roleService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<RoleDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<RoleDto>> GetById([FromRoute] int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Users.Create")]
        public override async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequest request)
        {
            try
            {
                return await base.Create(request).ConfigureAwait(false);
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<RoleDto>> Update(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                return await base.Update(id, request).ConfigureAwait(false);
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Settings.Delete")]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id).ConfigureAwait(false);
        }

        // ========== PERMISSION MANAGEMENT ENDPOINTS ==========

        /// <summary>
        /// GET /api/v1/Roles/permissions/catalog — Returns all available permissions.
        /// </summary>
        [HttpGet("permissions/catalog")]
        [Authorize(Policy = "Settings.View")]
        public async Task<ActionResult<List<PermissionDto>>> GetPermissionCatalog()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync().ConfigureAwait(false);
            return Ok(permissions);
        }

        /// <summary>
        /// GET /api/v1/Roles/{roleId}/permissions — Returns permission names assigned to a role.
        /// </summary>
        [HttpGet("{roleId}/permissions")]
        [Authorize(Policy = "Settings.View")]
        public async Task<ActionResult<List<string>>> GetRolePermissions([FromRoute] int roleId)
        {
            var permissions = await _permissionService.GetRolePermissionsAsync(roleId).ConfigureAwait(false);
            return Ok(permissions);
        }

        /// <summary>
        /// POST /api/v1/Roles/{roleId}/permissions — Assigns a permission to a role.
        /// </summary>
        [HttpPost("{roleId}/permissions")]
        [Authorize(Policy = "Settings.Edit")]
        public async Task<IActionResult> AssignPermission([FromRoute] int roleId, [FromBody] AssignPermissionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.AssignPermissionAsync(roleId, request.PermissionName).ConfigureAwait(false);

            if (!result)
                return Conflict(new { message = "No se pudo asignar el permiso. Verifique que el permiso exista y no esté ya asignado." });

            return Ok(new { message = "Permiso asignado correctamente." });
        }

        /// <summary>
        /// DELETE /api/v1/Roles/{roleId}/permissions/{permissionName} — Removes a permission from a role.
        /// Uses catch-all route parameter to handle dots in permission names (e.g., "Accidents.View").
        /// </summary>
        [HttpDelete("{roleId}/permissions/{*permissionName}")]
        [Authorize(Policy = "Settings.Edit")]
        public async Task<IActionResult> RemovePermission([FromRoute] int roleId, [FromRoute] string permissionName)
        {
            var result = await _permissionService.RemovePermissionAsync(roleId, permissionName).ConfigureAwait(false);

            if (!result)
                return NotFound(new { message = "No se encontró la asignación del permiso especificado." });

            return Ok(new { message = "Permiso removido correctamente." });
        }
    }
}
