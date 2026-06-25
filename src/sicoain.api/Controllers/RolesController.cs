using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.api.Exceptions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Roles;

namespace sicoain.api.Controllers
{
    public class RolesController : BaseCrudController<RoleDto, CreateRoleRequest, UpdateRoleRequest>
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService) : base(roleService)
        {
            _roleService = roleService;
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

    }
}
