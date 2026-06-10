
using sicoain.shared.DTOs;

namespace sicoain.client.Abstractions
{
    public interface IBaseService<TDto, TCreateRequest, TUpdateRequest>
         where TDto : class
        where TCreateRequest : class
        where TUpdateRequest : class
    {
        Task<PagedResponse<TDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<TDto?> GetByIdAsync(int id);
        Task<TDto> CreateAsync(TCreateRequest request);
        Task<TDto?> UpdateAsync(int id, TUpdateRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
