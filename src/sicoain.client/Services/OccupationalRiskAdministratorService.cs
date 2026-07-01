using sicoain.client.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.client.Services
{
    public class OccupationalRiskAdministratorService : BaseService<OccupationalRiskAdministratorDto, CreateOccupationalRiskAdministratorRequest, UpdateOccupationalRiskAdministratorRequest>,
        IOccupationalRiskAdministratorService
    {
        public OccupationalRiskAdministratorService(HttpClient httpClient)
            : base(httpClient, "OccupationalRiskAdministrators")
        {
        }

        // Shadow base methods to satisfy IOccupationalRiskAdministratorService with explicit interface types
        public new Task<PagedResponse<OccupationalRiskAdministratorDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
            => base.GetAllAsync(pageNumber, pageSize);

        public new Task<OccupationalRiskAdministratorDto?> GetByIdAsync(int id)
            => base.GetByIdAsync(id);

        public new Task<OccupationalRiskAdministratorDto> CreateAsync(CreateOccupationalRiskAdministratorRequest request)
            => base.CreateAsync(request);

        public new Task<bool> DeleteAsync(int id)
            => base.DeleteAsync(id);
    }
}
