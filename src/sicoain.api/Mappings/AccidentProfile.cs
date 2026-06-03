using AutoMapper;
using sicoain.shared.DTOs.Accident;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class AccidentProfile : Profile
    {
        public AccidentProfile()
        {
            // Entity -> DTO
            // The service already loads Employee, AccidentType, and EventCategory via .Include()
            CreateMap<Accident, AccidentDto>()
                .ForMember(dest => dest.EmployeeFullname, opt => opt.MapFrom(
                    src => src.Employee != null
                        ? $"{src.Employee.FirstName} {src.Employee.Surname}"
                        : null))
                .ForMember(dest => dest.AccidentTypeName, opt => opt.MapFrom(
                    src => src.AccidentType != null ? src.AccidentType.Name : null))
                .ForMember(dest => dest.EventCategoryName, opt => opt.MapFrom(
                    src => src.EventCategory != null ? src.EventCategory.Name : null));

            // Create Request -> Entity
            CreateMap<CreateAccidentRequest, Accident>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.AccidentType, opt => opt.Ignore())
                .ForMember(dest => dest.EventCategory, opt => opt.Ignore())
                .ForMember(dest => dest.DigitalEvidences, opt => opt.Ignore())
                .ForMember(dest => dest.Witnesses, opt => opt.Ignore())
                .ForMember(dest => dest.CorrectiveActions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // Update Request -> Entity
            CreateMap<UpdateAccidentRequest, Accident>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.AccidentType, opt => opt.Ignore())
                .ForMember(dest => dest.EventCategory, opt => opt.Ignore())
                .ForMember(dest => dest.DigitalEvidences, opt => opt.Ignore())
                .ForMember(dest => dest.Witnesses, opt => opt.Ignore())
                .ForMember(dest => dest.CorrectiveActions, opt => opt.Ignore())
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
