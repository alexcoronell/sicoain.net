using AutoMapper;
using sicoain.shared.DTOs.Permissions;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            // Entity -> DTO
            CreateMap<Permissions, PermissionDto>();
        }
    }
}
