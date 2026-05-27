using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Employees;

namespace sicoain.api.Controllers
{
    public class EmployeesController : BaseCrudController<EmployeeDto, CreateEmployeeRequest, UpdateEmployeeRequest>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService) : base(employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Policy = "Employees.View")]
        public override async Task<ActionResult<PagedResponse<EmployeeDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Employees.View")]
        public override async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Employees.Create")]
        public override async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Employees.Edit")]
        public override async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpdateEmployeeRequest request)
        {
            return await base.Update(id, request).ConfigureAwait(false);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Employees.Delete")]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id).ConfigureAwait(false);
        }
    }
}
