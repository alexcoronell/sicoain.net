using sicoain.shared.DTOs;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.client.Abstractions
{
    public interface IOccupationalRiskAdministratorService : IBaseService<OccupationalRiskAdministratorDto, CreateOccupationalRiskAdministratorRequest, UpdateOccupationalRiskAdministratorRequest>
    {
        new Task<PagedResponse<OccupationalRiskAdministratorDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        new Task<OccupationalRiskAdministratorDto?> GetByIdAsync(int id);
        new Task<OccupationalRiskAdministratorDto> CreateAsync(CreateOccupationalRiskAdministratorRequest request);
        Task<OccupationalRiskAdministratorDto?> UpdateAsync(int id, UpdateOccupationalRiskAdministratorRequest request);
        new Task<bool> DeleteAsync(int id);
    }
}
