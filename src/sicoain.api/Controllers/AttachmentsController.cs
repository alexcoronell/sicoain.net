using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs.Attachments;
using sicoain.shared.Enums;

namespace sicoain.api.Controllers
{
    [ApiController]
    [Authorize]
    public class AttachmentsController : BaseApiController
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentsController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            bool hasPermission = User.HasClaim("Permission", "Accidents.View") || User.HasClaim("Permission", "Employees.View");
            if (!hasPermission) return Forbid();
            var attachments = await _attachmentService.GetAllAsync(pageNumber, pageSize).ConfigureAwait(false);
            return Ok(attachments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var attachment = await _attachmentService.GetByIdAsync(id).ConfigureAwait(false);
            if (attachment == null) return NotFound();

            bool hasPermission = attachment.EntityType switch
            {
                AttachmentEntityType.Accident => User.HasClaim("Permission", "Accidents.View"),
                AttachmentEntityType.Employee => User.HasClaim("Permission", "Employees.View"),
                _ => false
            };
            if (!hasPermission) return Forbid();
            return Ok(attachment);
        }

        [HttpGet("by-entity")]
        public async Task<IActionResult> GetByEntityId([FromQuery] AttachmentEntityType entityType, [FromQuery] int id)
        {
            bool hasPermission = entityType switch
            {
                AttachmentEntityType.Accident => User.HasClaim("Permission", "Accidents.View"),
                AttachmentEntityType.Employee => User.HasClaim("Permission", "Employees.View"),
                _ => false
            };
            if (!hasPermission) return Forbid();
            var attachments = await _attachmentService.GetByEntityIdAsync(entityType, id).ConfigureAwait(false);
            return Ok(attachments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateAttachmentRequest request)
        {
            bool hasPermission = request.EntityType switch
            {
                AttachmentEntityType.Accident => User.HasClaim("Permission", "Accidents.Create"),
                AttachmentEntityType.Employee => User.HasClaim("Permission", "Employees.Create"),
                _ => false
            };
            if (!hasPermission) return Forbid();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _attachmentService.UploadAsync(request).ConfigureAwait(false);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMetadataAsync(int id, [FromForm] UpdateAttachmentRequest request)
        {

            var attachment = await _attachmentService.GetByIdAsync(id).ConfigureAwait(false);
            if (attachment == null) return NotFound();

            bool hasPermission = attachment.EntityType switch
            {
                AttachmentEntityType.Accident => User.HasClaim("Permission", "Accidents.Edit"),
                AttachmentEntityType.Employee => User.HasClaim("Permission", "Employees.Edit"),
                _ => false
            };
            if (!hasPermission) return Forbid();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _attachmentService.UpdateMetadataAsync(id, request).ConfigureAwait(false);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var attachment = await _attachmentService.GetByIdAsync(id).ConfigureAwait(false);
            if (attachment == null) return NotFound();

            bool hasPermission = attachment.EntityType switch
            {
                AttachmentEntityType.Accident => User.HasClaim("Permission", "Accidents.Edit"),
                AttachmentEntityType.Employee => User.HasClaim("Permission", "Employees.Edit"),
                _ => false
            };
            if (!hasPermission) return Forbid();

            try
            {
                await _attachmentService.DeleteAsync(id).ConfigureAwait(false);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

    }
}
