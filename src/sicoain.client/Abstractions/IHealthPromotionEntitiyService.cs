using sicoain.shared.DTOs;
using sicoain.shared.DTOs.HealthPromotionEntities;

namespace sicoain.client.Abstractions
{
    public interface IHealthPromotionEntitiyService: IBaseService<HealthPromotionEntityDto, CreateHealthPromotionEntityRequest, UpdateHealthPromotionEntityRequest>
    {
        new Task <PagedResponse<HealthPromotionEntityDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        new Task<HealthPromotionEntityDto?> GetByIdAsync(int id);
        new Task<HealthPromotionEntityDto> CreateAsync(CreateHealthPromotionEntityRequest request);
        Task<HealthPromotionEntityDto?> UpdateAsync(int id, UpdateHealthPromotionEntityRequest request);
        new Task<bool> DeleteAsync(int id);
    }
}
