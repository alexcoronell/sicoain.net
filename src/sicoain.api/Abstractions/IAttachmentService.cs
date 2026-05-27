using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Attachments;
using sicoain.shared.Enums;

namespace sicoain.api.Abstractions
{
    public interface IAttachmentService
    {
        Task<PagedResponse<AttachmentDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<AttachmentDto?> GetByIdAsync(int id);

        Task<IEnumerable<AttachmentDto>> GetByEntityIdAsync(AttachmentEntityType entityType, int id);
        Task<AttachmentDto> UploadAsync(CreateAttachmentRequest request);
        Task<AttachmentDto> UpdateMetadataAsync(int id, UpdateAttachmentRequest request);
        Task DeleteAsync(int id);
    }
}
