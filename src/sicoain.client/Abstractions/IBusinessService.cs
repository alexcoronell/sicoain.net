using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Business;

namespace sicoain.client.Abstractions
{
    public interface IBusinessService : IBaseService<BusinessDto, CreateBusinessRequest, UpdateBusinessEmailRequest>
    {
        new Task <PagedResponse<BusinessDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        new Task<BusinessDto?> GetByIdAsync(int id);
        new Task<BusinessDto> CreateAsync(CreateBusinessRequest request);
        Task<BusinessDto?> UpdateAsync(int id, UpdateBusinessRequest request);
        new Task<bool> DeleteAsync(int id);
    }
}
