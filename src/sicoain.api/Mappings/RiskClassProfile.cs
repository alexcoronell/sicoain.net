using AutoMapper;
using sicoain.shared.DTOs.RiskClasses;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class RiskClassProfile : Profile
    {
        public RiskClassProfile()
        {
            // Entity -> DTO
            CreateMap<RiskClass, RiskClassDto>();

            // Create Request -> Entity
            CreateMap<CreateRiskClassRequest, RiskClass>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Positions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // Update Request -> Entity
            CreateMap<UpdateRiskClassRequest, RiskClass>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Positions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                // NOTE: ForAllMembers runs AFTER ForMember and overrides explicit conditions.
                // Use explicit ForMember conditions for each property with proper null checks.
                .ForMember(dest => dest.Name, opt => opt.Condition((src, _, _) => src.Name is not null))
                .ForMember(dest => dest.Code, opt => opt.Condition((src, _, _) => src.Code is not null))
                .ForMember(dest => dest.ContributionRate, opt => opt.Condition((src, _, _) => src.ContributionRate.HasValue))
                .ForMember(dest => dest.IsActive, opt => opt.Condition((src, _, _) => src.IsActive.HasValue));
        }
    }
}
