using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.Attachments
{
    public record AttachmentDto : BaseDto
    {
        public string FileName { get; init; } = string.Empty;
        public string FilePath { get; init; } = string.Empty;
        public long FileSize { get; init; }
        public string MimeType { get; init; } = string.Empty;
        public string FileHash { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public AttachmentEntityType EntityType { get; init; }
        public int EntityId { get; init; }

    }
}
