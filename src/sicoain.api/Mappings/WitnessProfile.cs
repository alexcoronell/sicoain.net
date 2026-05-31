using AutoMapper;
using sicoain.shared.DTOs.Witnesses;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class WitnessProfile : Profile
    {
        public WitnessProfile()
        {
            // Entity -> DTO
            // Note: WitnessDto does not extend BaseDto (no Id, timestamps, etc.)
            // EmployeeFullname is resolved from the loaded Employee nav property
            // The service already includes .Include(e => e.Employee) and .Include(e => e.Accident)
            CreateMap<Witness, WitnessDto>()
                .ForMember(dest => dest.EmployeeFullname, opt => opt.MapFrom(
                    src => src.Employee != null
                        ? $"{src.Employee.FirstName} {src.Employee.Surname}"
                        : null));

            // Create Request -> Entity
            CreateMap<CreateWitnessRequest, Witness>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Accident, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // Update Request -> Entity
            CreateMap<UpdateWitnessRequest, Witness>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Accident, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
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
