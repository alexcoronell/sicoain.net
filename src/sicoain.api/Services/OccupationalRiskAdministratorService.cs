using AutoMapper;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class OccupationalRiskAdministratorService : BaseService<OccupationalRiskAdministrator, OccupationalRiskAdministratorDto, CreateOccupationalRiskAdministratorRequest, UpdateOccupationalRiskAdministratorRequest>, IOccupationalRiskAdministratorService
    {
        public OccupationalRiskAdministratorService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
