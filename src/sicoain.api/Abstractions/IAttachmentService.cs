using sicoain.shared.DTOs.Attachments;
using sicoain.shared.Enums;

namespace sicoain.api.Abstractions
{
    public interface IAttachmentService
    {
        Task<AttachmentDto> GetByIdAsync(int id);

        Task<IEnumerable<AttachmentDto>> GetByEntityIdAsync(AttachmentEntityType entityType, int entityId);
        Task<AttachmentDto> UploadAsync(CreateAttachmentRequest request);
        Task<AttachmentDto> UpdateMetadataAsync(int id, UpdateAttachmentRequest request);
        Task DeleteAsync(int id);
        Task<IEnumerable<AttachmentDto>> GetAllAsync();
    }
}
