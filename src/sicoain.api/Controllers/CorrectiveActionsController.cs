using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.CorrectiveActions;

namespace sicoain.api.Controllers
{
    public class CorrectiveActionsController : BaseCrudController<CorrectiveActionDto, CreateCorrectiveActionRequest, UpdateCorrectiveActionRequest>
    {
        private readonly ICorrectiveActionService _correctiveActionService;

        public CorrectiveActionsController(ICorrectiveActionService correctiveActionService) : base(correctiveActionService)
        {
            _correctiveActionService = correctiveActionService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<CorrectiveActionDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<CorrectiveActionDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<CorrectiveActionDto>> Create([FromBody] CreateCorrectiveActionRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<CorrectiveActionDto>> Update(int id, [FromBody] UpdateCorrectiveActionRequest request)
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
