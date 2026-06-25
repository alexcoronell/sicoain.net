using sicoain.client.Abstractions;
using sicoain.shared.DTOs.Roles;

namespace sicoain.client.Services
{
    public class RoleService : BaseService<RoleDto, CreateRoleRequest, UpdateRoleRequest>, IRoleService
    {
        public RoleService(HttpClient httpClient) : base(httpClient, "role")
        {

        }
    }
}
