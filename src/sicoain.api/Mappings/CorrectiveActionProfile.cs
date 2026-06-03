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
            // NOTE: ForAllMembers runs AFTER ForMember and overrides explicit conditions.
            // Use explicit ForMember conditions for each value-type property to avoid the
            // AutoMapper nullable-value-type bug where int? null → 0 before the null check.
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
                .ForMember(dest => dest.Title, opt => opt.Condition((src, _, _) => src.Title is not null))
                .ForMember(dest => dest.Description, opt => opt.Condition((src, _, _) => src.Description is not null))
                .ForMember(dest => dest.DueDate, opt => opt.Condition((src, _, _) => src.DueDate.HasValue))
                .ForMember(dest => dest.Status, opt => opt.Condition((src, _, _) => src.Status.HasValue))
                .ForMember(dest => dest.Priority, opt => opt.Condition((src, _, _) => src.Priority.HasValue))
                .ForMember(dest => dest.AccidentId, opt => opt.Condition((src, _, _) => src.AccidentId.HasValue));
        }
    }
}
