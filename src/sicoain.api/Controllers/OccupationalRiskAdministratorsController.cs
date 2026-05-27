using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.api.Controllers
{
    public class OccupationalRiskAdministratorsController : BaseCrudController<OccupationalRiskAdministratorDto, CreateOccupationalRiskAdministratorRequest, UpdateOccupationalRiskAdministratorRequest>
    {
        private readonly IOccupationalRiskAdministratorService _occupationalRiskAdministratorService;

        public OccupationalRiskAdministratorsController(IOccupationalRiskAdministratorService occupationalRiskAdministratorService) : base(occupationalRiskAdministratorService)
        {
            _occupationalRiskAdministratorService = occupationalRiskAdministratorService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<OccupationalRiskAdministratorDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<OccupationalRiskAdministratorDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<OccupationalRiskAdministratorDto>> Create([FromBody] CreateOccupationalRiskAdministratorRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<OccupationalRiskAdministratorDto>> Update(int id, [FromBody] UpdateOccupationalRiskAdministratorRequest request)
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
