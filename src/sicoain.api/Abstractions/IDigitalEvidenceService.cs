using sicoain.shared.DTOs;
using sicoain.shared.DTOs.DigitalEvidences;

namespace sicoain.api.Abstractions
{
    public interface IDigitalEvidenceService
    {
        Task<PagedResponse<DigitalEvidenceDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<DigitalEvidenceDto?> GetByIdAsync(int id);
        Task<IEnumerable<DigitalEvidenceDto>> GetByAccidentIdAsync(int accidentId);
        Task<DigitalEvidenceDto> UploadAsync(CreateDigitalEvidenceRequest request);
        Task<DigitalEvidenceDto> UpdateMetadataAsync(int id, UpdateDigitalEvidenceRequest request);
        Task DeleteAsync(int id);
    }
}
