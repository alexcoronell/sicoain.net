using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Accident;

namespace sicoain.api.Controllers
{
    public class AccidentsController : BaseCrudController<AccidentDto, CreateAccidentRequest, UpdateAccidentRequest>
    {
        private readonly IAccidentService _accidentService;

        public AccidentsController(IAccidentService accidentService) : base(accidentService)
        {
            _accidentService = accidentService;
        }

        [HttpGet]
        [Authorize(Policy = "Accidents.View")]
        public override async Task<ActionResult<PagedResponse<AccidentDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Accidents.View")]
        public override async Task<ActionResult<AccidentDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Accidents.Create")]
        public override async Task<ActionResult<AccidentDto>> Create([FromBody] CreateAccidentRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Accidents.Edit")]
        public override async Task<ActionResult<AccidentDto>> Update(int id, [FromBody] UpdateAccidentRequest request)
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
