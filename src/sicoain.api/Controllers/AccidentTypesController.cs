using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.AccidentTypes;

namespace sicoain.api.Controllers
{
    public class AccidentTypesController : BaseCrudController<AccidentTypeDto, CreateAccidentTypeRequest, UpdateAccidentTypeRequest>
    {
        private readonly IAccidentTypeService _accidentTypeService;

        public AccidentTypesController(IAccidentTypeService accidentTypeService) : base(accidentTypeService)
        {
            _accidentTypeService = accidentTypeService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<AccidentTypeDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<AccidentTypeDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<AccidentTypeDto>> Create([FromBody] CreateAccidentTypeRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<AccidentTypeDto>> Update(int id, [FromBody] UpdateAccidentTypeRequest request)
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
