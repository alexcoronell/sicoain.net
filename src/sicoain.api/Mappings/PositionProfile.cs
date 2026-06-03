using AutoMapper;
using sicoain.shared.DTOs.Positions;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class PositionProfile : Profile
    {
        public PositionProfile()
        {
            // Entity -> DTO
            CreateMap<Position, PositionDto>();

            // Create Request -> Entity
            CreateMap<CreatePositionRequest, Position>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.RiskClass, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // Update Request -> Entity
            CreateMap<UpdatePositionRequest, Position>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.RiskClass, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                // NOTE: ForAllMembers runs AFTER ForMember and overrides explicit conditions.
                // Use explicit ForMember conditions for each value-type property.
                .ForMember(dest => dest.Name, opt => opt.Condition((src, _, _) => src.Name is not null))
                .ForMember(dest => dest.Description, opt => opt.Condition((src, _, _) => src.Description is not null))
                .ForMember(dest => dest.DepartmentId, opt => opt.Condition((src, _, _) => src.DepartmentId.HasValue))
                .ForMember(dest => dest.RiskClassId, opt => opt.Condition((src, _, _) => src.RiskClassId.HasValue));
        }
    }
}
