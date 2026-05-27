using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Branches;

namespace sicoain.api.Controllers
{
    public class BranchesController : BaseCrudController<BranchDto, CreateBranchRequest, UpdateBranchRequest>
    {
        private readonly IBranchService _branchesService;

        public BranchesController(IBranchService branchesService) : base(branchesService)
        {
            _branchesService = branchesService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<BranchDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<BranchDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<BranchDto>> Create([FromBody] CreateBranchRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<BranchDto>> Update(int id, [FromBody] UpdateBranchRequest request)
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
