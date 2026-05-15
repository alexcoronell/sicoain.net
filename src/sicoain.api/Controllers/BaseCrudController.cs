using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;

namespace sicoain.api.Controllers
{
    public abstract class BaseCrudController<TDto, TCreateRequest, TUpdateRequest> : BaseApiController
        where TDto : class
        where TCreateRequest : class
        where TUpdateRequest : class
    {
        protected readonly IBaseService<TDto, TCreateRequest, TUpdateRequest> _service;
        protected BaseCrudController(IBaseService<TDto, TCreateRequest, TUpdateRequest> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<ActionResult<PagedResponse<TDto>>> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id).ConfigureAwait(false);

            if (dto == null) return NotFound();

            return Ok(dto);
        }

        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(request).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetById), new { id = GetId(created) }, created);
        }

        [HttpPatch("{id}")]
        public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TUpdateRequest request)
        {
            var exists = await GetById(id).ConfigureAwait(false);
            if (exists == null) return NotFound();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, request).ConfigureAwait(false);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id).ConfigureAwait(false);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // Helper to extract Id from DTO
        private int GetId(TDto dto)
        {
            var property = dto?.GetType().GetProperty("Id");
            if (property == null) return 0;
            var value = property.GetValue(dto);
            return value is int id ? id : 0;
        }
    }
}
