using AutoMapper;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.RiskClasses;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class RiskClassService : BaseService<RiskClass, RiskClassDto, CreateRiskClassRequest, UpdateRiskClassRequest>, IRiskClassService
    {
        public RiskClassService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
