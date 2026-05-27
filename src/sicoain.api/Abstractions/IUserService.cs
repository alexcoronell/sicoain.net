using sicoain.shared.DTOs.Users;

namespace sicoain.api.Abstractions
{
    public interface IUserService : IBaseService<UserDto, CreateUserRequest, UpdateUserRequest>
    {
        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        Task<UserDto?> GetByEmailAsync(string email);

        /// <summary>
        /// Checks if a user exists with the given email
        /// </summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        Task<bool> AssignRoleAsync(int userId, string roleName);

        /// <summary>
        /// Removes a role from a user
        /// </summary>
        Task<bool> RemoveRoleAsync(int userId, string roleName);

        /// <summary>
        /// Changes user's password
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);

        /// <summary>
        /// Gets all roles assigned to a user
        /// </summary>
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    }
}
