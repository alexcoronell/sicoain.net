using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Witnesses;

namespace sicoain.api.Controllers
{
    public class WitnessesController : BaseCrudController<WitnessDto, CreateWitnessRequest, UpdateWitnessRequest>
    {
        private readonly IWitnessService _witnessService;

        public WitnessesController(IWitnessService witnessService) : base(witnessService)
        {
            _witnessService = witnessService;
        }

        [HttpGet]
        [Authorize(Policy = "Accidents.View")]
        public override async Task<ActionResult<PagedResponse<WitnessDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Accidents.View")]
        public override async Task<ActionResult<WitnessDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Accidents.Create")]
        public override async Task<ActionResult<WitnessDto>> Create([FromBody] CreateWitnessRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Accidents.Edit")]
        public override async Task<ActionResult<WitnessDto>> Update(int id, [FromBody] UpdateWitnessRequest request)
        {
            return await base.Update(id, request).ConfigureAwait(false);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Accidents.Delete")]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id).ConfigureAwait(false);
        }
    }
}
