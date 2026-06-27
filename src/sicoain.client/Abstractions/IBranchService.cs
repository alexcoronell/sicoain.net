using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Branches;

namespace sicoain.client.Abstractions
{
    public interface IBranchService : IBaseService<BranchDto, CreateBranchRequest, UpdateBranchRequest>
    {
        new Task<PagedResponse<BranchDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        new Task<BranchDto?> GetByIdAsync(int id);
        new Task<BranchDto> CreateAsync(CreateBranchRequest request);
        new Task<BranchDto?> UpdateAsync(int id, UpdateBranchRequest request);
        new Task<bool> DeleteAsync(int id);
    }
}
