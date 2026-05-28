using System.ComponentModel.DataAnnotations;
using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.Attachments
{
    public record CreateAttachmentRequest
    {
        [Required]
        public required string FileName { get; init; }

        [Required]
        public required string MimeType { get; init; }

        [Required]
        public required string Description { get; init; } = string.Empty;

        [Required]
        public required AttachmentEntityType EntityType { get; init; }

        [Required]
        public int EntityId { get; init; }

        [Required]
        public required string Base64Content { get; init; }
    }
}
