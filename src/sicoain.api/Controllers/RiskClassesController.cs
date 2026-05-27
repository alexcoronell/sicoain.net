using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.RiskClasses;

namespace sicoain.api.Controllers
{
    public class RiskClassesController : BaseCrudController<RiskClassDto, CreateRiskClassRequest, UpdateRiskClassRequest>
    {
        private readonly IRiskClassService _riskClassService;

        public RiskClassesController(IRiskClassService riskClassService) : base(riskClassService)
        {
            _riskClassService = riskClassService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<RiskClassDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<RiskClassDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<RiskClassDto>> Create([FromBody] CreateRiskClassRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<RiskClassDto>> Update(int id, [FromBody] UpdateRiskClassRequest request)
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
