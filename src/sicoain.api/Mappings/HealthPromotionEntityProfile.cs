using AutoMapper;
using sicoain.shared.DTOs.HealthPromotionEntities;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class HealthPromotionEntityProfile : Profile
    {
        public HealthPromotionEntityProfile()
        {
            // ========================================================================
            // HealthPromotionEntity (entity) -> HealthPromotionEntityDto
            // ========================================================================
            CreateMap<HealthPromotionEntity, HealthPromotionEntityDto>();

            // ========================================================================
            // CreateHealthPromotionEntityRequest -> HealthPromotionEntity (entity)
            // ========================================================================
            CreateMap<CreateHealthPromotionEntityRequest, HealthPromotionEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // ========================================================================
            // UpdateHealthPromotionEntityRequest -> HealthPromotionEntity (entity)
            // ========================================================================
            CreateMap<UpdateHealthPromotionEntityRequest, HealthPromotionEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========================================================================
            // HealthPromotionEntityPhone (entity) -> HealthPromotionEntityPhoneDto
            // ========================================================================
            CreateMap<HealthPromotionEntityPhone, HealthPromotionEntityPhoneDto>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone));

            // ========================================================================
            // CreateHealthPromotionEntityPhoneRequest -> HealthPromotionEntityPhone (entity)
            // ========================================================================
            CreateMap<CreateHealthPromotionEntityPhoneRequest, HealthPromotionEntityPhone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.PhoneType, opt => opt.Ignore())
                .ForMember(dest => dest.HealthPromotionEntity, opt => opt.Ignore());

            // ========================================================================
            // UpdateHealthPromotionEntityPhoneRequest -> HealthPromotionEntityPhone (entity)
            // ========================================================================
            CreateMap<UpdateHealthPromotionEntityPhoneRequest, HealthPromotionEntityPhone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneType, opt => opt.Ignore())
                .ForMember(dest => dest.HealthPromotionEntityId, opt => opt.Ignore())
                .ForMember(dest => dest.HealthPromotionEntity, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========================================================================
            // HealthPromotionEntityEmail (entity) -> HealthPromotionEntityEmailDto
            // ========================================================================
            CreateMap<HealthPromotionEntityEmail, HealthPromotionEntityEmailDto>();

            // ========================================================================
            // CreateHealthPromotionEntityEmailRequest -> HealthPromotionEntityEmail (entity)
            // ========================================================================
            CreateMap<CreateHealthPromotionEntityEmailRequest, HealthPromotionEntityEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.HealthPromotionEntity, opt => opt.Ignore());

            // ========================================================================
            // UpdateHealthPromotionEntityEmailRequest -> HealthPromotionEntityEmail (entity)
            // ========================================================================
            CreateMap<UpdateHealthPromotionEntityEmailRequest, HealthPromotionEntityEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.HealthPromotionEntityId, opt => opt.Ignore())
                .ForMember(dest => dest.HealthPromotionEntity, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
