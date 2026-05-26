using AutoMapper;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.HealthPromotionEntities;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class HealthPromotionEntityService : BaseService<HealthPromotionEntity, HealthPromotionEntityDto, CreateHealthPromotionEntityRequest, UpdateHealthPromotionEntityRequest>, IHealthPromotionEntityService
    {
        public HealthPromotionEntityService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
