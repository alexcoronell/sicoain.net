using sicoain.client.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.HealthPromotionEntities;

namespace sicoain.client.Services
{
    public class HealthPromotionEntitiyService: BaseService<HealthPromotionEntityDto, CreateHealthPromotionEntityRequest, UpdateHealthPromotionEntityRequest>,
        IHealthPromotionEntitiyService
    {
        public HealthPromotionEntitiyService(HttpClient httpClient)
            : base(httpClient, "health-promotion-entities")
        {
        }

        // Shadow base methods to satisfy IHealthPromotionEntitiyService with explicit interface types
        public new Task<PagedResponse<HealthPromotionEntityDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
            => base.GetAllAsync(pageNumber, pageSize);

        public new Task<HealthPromotionEntityDto?> GetByIdAsync(int id)
            => base.GetByIdAsync(id);

        public new Task<HealthPromotionEntityDto> CreateAsync(CreateHealthPromotionEntityRequest request)
            => base.CreateAsync(request);

        public new Task<bool> DeleteAsync(int id)
            => base.DeleteAsync(id);
    }
}
