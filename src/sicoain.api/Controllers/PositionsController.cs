using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Positions;

namespace sicoain.api.Controllers
{
    public class PositionsController : BaseCrudController<PositionDto, CreatePositionRequest, UpdatePositionRequest>
    {
        private readonly IPositionService _positionService;

        public PositionsController(IPositionService positionService) : base(positionService)
        {
            _positionService = positionService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<PositionDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PositionDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<PositionDto>> Create([FromBody] CreatePositionRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<PositionDto>> Update(int id, [FromBody] UpdatePositionRequest request)
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
