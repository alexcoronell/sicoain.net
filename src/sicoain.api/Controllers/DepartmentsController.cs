using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Departments;

namespace sicoain.api.Controllers
{
    public class DepartmentsController : BaseCrudController<DepartmentDto, CreateDepartmentRequest, UpdateDepartmentRequest>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService) : base(departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<DepartmentDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<DepartmentDto>> Update(int id, [FromBody] UpdateDepartmentRequest request)
        {
            return await base.Update(id, request).ConfigureAwait(false);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id).ConfigureAwait(false);
        }
    }
}
