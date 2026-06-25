using sicoain.shared.DTOs.Permissions;

namespace sicoain.client.Abstractions
{
    public interface IPermissionService
    {
        Task<List<PermissionDto>> GetAllAsync();
        Task<List<string>> GetRolePermissionsAsync(int roleId);
        Task<bool> AssignPermissionAsync(int roleId, string permissionName);
        Task<bool> RemovePermissionAsync(int roleId, string permissionName);
    }
}
