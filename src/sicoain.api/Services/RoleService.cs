using AutoMapper;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.Roles;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class RoleService : BaseService<Roles, RoleDto, CreateRoleRequest, UpdateRoleRequest>, IRoleService
    {
        public RoleService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {

        }
    }
}
