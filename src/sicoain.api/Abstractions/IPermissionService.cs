using System.Security.Claims;
using sicoain.shared.DTOs.Permissions;
using sicoain.shared.Entities;

namespace sicoain.api.Abstractions
{
    public interface IPermissionService
    {
        /// <summary>
        /// Gets the list of permission names (e.g., "Accidents.View") for a given user.
        /// </summary>
        Task<List<string>> GetUserPermissionNameAsync(User user);

        /// <summary>
        /// Gets all non-deleted permissions from the catalog.
        /// </summary>
        Task<List<PermissionDto>> GetAllPermissionsAsync();

        /// <summary>
        /// Gets the list of permission names assigned to a specific role.
        /// </summary>
        Task<List<string>> GetRolePermissionsAsync(int roleId);

        /// <summary>
        /// Assigns a permission to a role.
        /// </summary>
        Task<bool> AssignPermissionAsync(int roleId, string permissionName);

        /// <summary>
        /// Removes (soft-deletes) a permission from a role.
        /// </summary>
        Task<bool> RemovePermissionAsync(int roleId, string permissionName);
    }
}
