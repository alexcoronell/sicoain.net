using AutoMapper;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class OccupationalRiskAdministratorProfile : Profile
    {
        public OccupationalRiskAdministratorProfile()
        {
            // ========================================================================
            // OccupationalRiskAdministrator (entity) -> OccupationalRiskAdministratorDto
            // Note: DTO does NOT extend BaseDto (no Id, timestamps, etc.)
            // ========================================================================
            CreateMap<OccupationalRiskAdministrator, OccupationalRiskAdministratorDto>();

            // ========================================================================
            // CreateOccupationalRiskAdministratorRequest -> OccupationalRiskAdministrator (entity)
            // Note: CreateRequest extends BaseDto (unusual), so we ignore BaseDto fields
            // ========================================================================
            CreateMap<CreateOccupationalRiskAdministratorRequest, OccupationalRiskAdministrator>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Phones, opt => opt.Ignore())
                .ForMember(dest => dest.Emails, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // ========================================================================
            // UpdateOccupationalRiskAdministratorRequest -> OccupationalRiskAdministrator (entity)
            // ========================================================================
            CreateMap<UpdateOccupationalRiskAdministratorRequest, OccupationalRiskAdministrator>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Phones, opt => opt.Ignore())
                .ForMember(dest => dest.Emails, opt => opt.Ignore())
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
            // OccupationalRiskAdministratorPhone (entity) -> OccupationalRiskAdministratorPhoneDto
            // ========================================================================
            CreateMap<OccupationalRiskAdministratorPhone, OccupationalRiskAdministratorPhoneDto>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone));

            // ========================================================================
            // CreateOccupationalRiskAdministratorPhoneRequest -> OccupationalRiskAdministratorPhone (entity)
            // ========================================================================
            CreateMap<CreateOccupationalRiskAdministratorPhoneRequest, OccupationalRiskAdministratorPhone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.OccupationalRiskAdministrator, opt => opt.Ignore());

            // ========================================================================
            // UpdateOccupationalRiskAdministratorPhoneRequest -> OccupationalRiskAdministratorPhone (entity)
            // ========================================================================
            CreateMap<UpdateOccupationalRiskAdministratorPhoneRequest, OccupationalRiskAdministratorPhone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.OccupationalRiskAdministratorId, opt => opt.Ignore())
                .ForMember(dest => dest.OccupationalRiskAdministrator, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========================================================================
            // OccupationalRiskAdministratorEmail (entity) -> OccupationalRiskAdministratorEmailDto
            // ========================================================================
            CreateMap<OccupationalRiskAdministratorEmail, OccupationalRiskAdministratorEmailDto>();

            // ========================================================================
            // CreateOccupationalRiskAdministratorEmailRequest -> OccupationalRiskAdministratorEmail (entity)
            // ========================================================================
            CreateMap<CreateOccupationalRiskAdministratorEmailRequest, OccupationalRiskAdministratorEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.OccupationalRiskAdministrator, opt => opt.Ignore());

            // ========================================================================
            // UpdateOccupationalRiskAdministratorEmailRequest -> OccupationalRiskAdministratorEmail (entity)
            // ========================================================================
            CreateMap<UpdateOccupationalRiskAdministratorEmailRequest, OccupationalRiskAdministratorEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.OccupationalRiskAdministratorId, opt => opt.Ignore())
                .ForMember(dest => dest.OccupationalRiskAdministrator, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
