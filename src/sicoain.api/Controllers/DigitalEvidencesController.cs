using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs.DigitalEvidences;

namespace sicoain.api.Controllers
{
    [ApiController]
    [Authorize]
    public class DigitalEvidencesController : BaseApiController
    {
        private readonly IDigitalEvidenceService _digitalEvidenceService;

        public DigitalEvidencesController(IDigitalEvidenceService digitalEvidenceService)
        {
            _digitalEvidenceService = digitalEvidenceService;
        }

        [HttpGet]
        [Authorize(Policy = "Accidents.View")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            var digitalEvidences = await _digitalEvidenceService.GetAllAsync(pageNumber, pageSize).ConfigureAwait(false);
            return Ok(digitalEvidences);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Accidents.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var digitalEvidence = await _digitalEvidenceService.GetByIdAsync(id).ConfigureAwait(false);
            if (digitalEvidence == null) return NotFound();
            return Ok(digitalEvidence);
        }

        [HttpGet("by-accident/{accidentId}")]
        [Authorize(Policy = "Accidents.View")]
        public async Task<IActionResult> GetByAccidentId(int accidentId)
        {
            var digitalEvidences = await _digitalEvidenceService.GetByAccidentIdAsync(accidentId).ConfigureAwait(false);
            return Ok(digitalEvidences);
        }

        [HttpPost]
        [Authorize(Policy = "Accidents.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateDigitalEvidenceRequest request)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _digitalEvidenceService.UploadAsync(request).ConfigureAwait(false);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "Accidents.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMetadataAsync(int id, [FromForm] UpdateDigitalEvidenceRequest request)
        {

            var digitalEvidence = await _digitalEvidenceService.GetByIdAsync(id).ConfigureAwait(false);
            if (digitalEvidence == null) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _digitalEvidenceService.UpdateMetadataAsync(id, request).ConfigureAwait(false);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Accidents.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var digitalEvidence = await _digitalEvidenceService.GetByIdAsync(id).ConfigureAwait(false);
            if (digitalEvidence == null) return NotFound();

            try
            {
                await _digitalEvidenceService.DeleteAsync(id).ConfigureAwait(false);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
