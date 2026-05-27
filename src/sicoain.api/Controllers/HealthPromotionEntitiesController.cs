using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.HealthPromotionEntities;
namespace sicoain.api.Controllers
{
    public class HealthPromotionEntitiesController : BaseCrudController<HealthPromotionEntityDto, CreateHealthPromotionEntityRequest, UpdateHealthPromotionEntityRequest>
    {
        private readonly IHealthPromotionEntityService _healthPromotionEntityService;

        public HealthPromotionEntitiesController(IHealthPromotionEntityService healthPromotionEntityService) : base(healthPromotionEntityService)
        {
            _healthPromotionEntityService = healthPromotionEntityService;
        }

        [HttpGet]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<PagedResponse<HealthPromotionEntityDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            return await base.GetAll(pageNumber, pageSize).ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Settings.View")]
        public override async Task<ActionResult<HealthPromotionEntityDto>> GetById(int id)
        {
            return await base.GetById(id).ConfigureAwait(false);
        }

        [HttpPost]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<HealthPromotionEntityDto>> Create([FromBody] CreateHealthPromotionEntityRequest request)
        {
            return await base.Create(request).ConfigureAwait(false);

        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Settings.Edit")]
        public override async Task<ActionResult<HealthPromotionEntityDto>> Update(int id, [FromBody] UpdateHealthPromotionEntityRequest request)
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
