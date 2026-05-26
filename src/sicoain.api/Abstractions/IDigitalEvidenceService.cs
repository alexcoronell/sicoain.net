using sicoain.shared.DTOs.DigitalEvidences;

namespace sicoain.api.Abstractions
{
    public interface IDigitalEvidenceService
    {
        Task<DigitalEvidenceDto> GetByIdAsync(int id);

        Task<IEnumerable<DigitalEvidenceDto>> GetByEntityIdAsync(int entityId);
        Task<DigitalEvidenceDto> UploadAsync(CreateDigitalEvidenceRequest request);
        Task<DigitalEvidenceDto> UpdateMetadataAsync(int id, UpdateDigitalEvidenceRequest request);
        Task DeleteAsync(int id);
        Task<IEnumerable<DigitalEvidenceDto>> GetAllAsync();
    }
}
