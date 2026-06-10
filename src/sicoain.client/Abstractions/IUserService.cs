using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Users;

namespace sicoain.client.Abstractions
{
    public interface IUserService : IBaseService<UserDto, CreateUserRequest, UpdateUserRequest>
    {
        new Task<PagedResponse<UserDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        new Task<UserDto?> GetByIdAsync(int id);
        new Task<UserDto> CreateAsync(CreateUserRequest request);
        new Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request);
        new Task<bool> DeleteAsync(int id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> AssignRoleAsync(int userId, string roleName);
        Task<bool> RemoveRoleAsync(int userId, string roleName);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    }
}
