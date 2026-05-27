

using System.ComponentModel.DataAnnotations;
using sicoain.shared.Enums;

namespace sicoain.shared.Entities
{
    public class Attachment : BaseEntity
    {
        [Required]
        public required string FileName { get; set; }

        [Required]
        public required string FilePath { get; set; }

        [Required]
        public required long FileSize { get; set; }

        [Required]
        public required string MimeType { get; set; }

        [Required]
        public required string FileHash { get; set; }

        [Required]
        public required string Description { get; set; } = string.Empty;

        [Required]
        public required AttachmentEntityType EntityType { get; set; }

        [Required]
        public int EntityId { get; set; }
    }
}
