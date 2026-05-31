using AutoMapper;
using sicoain.shared.DTOs.CorrectiveActions;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class CorrectiveActionProfile : Profile
    {
        public CorrectiveActionProfile()
        {
            // Entity -> DTO
            CreateMap<CorrectiveAction, CorrectiveActionDto>();

            // Create Request -> Entity
            CreateMap<CreateCorrectiveActionRequest, CorrectiveAction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompletionDate, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationNotes, opt => opt.Ignore())
                .ForMember(dest => dest.IsEffective, opt => opt.Ignore())
                .ForMember(dest => dest.Trackings, opt => opt.Ignore())
                .ForMember(dest => dest.Accident, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // Update Request -> Entity
            CreateMap<UpdateCorrectiveActionRequest, CorrectiveAction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompletionDate, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationNotes, opt => opt.Ignore())
                .ForMember(dest => dest.IsEffective, opt => opt.Ignore())
                .ForMember(dest => dest.Trackings, opt => opt.Ignore())
                .ForMember(dest => dest.Accident, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
