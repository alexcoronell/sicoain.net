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
            // NOTE: ForAllMembers runs AFTER ForMember and overrides explicit conditions.
            // Use explicit ForMember conditions for each value-type property to avoid the
            // AutoMapper nullable-value-type bug where int? null → 0 before the null check.
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
                .ForMember(dest => dest.AccidentId, opt => opt.Condition((src, _, _) => src.AccidentId.HasValue))
                .ForMember(dest => dest.EmployeeId, opt => opt.Condition((src, _, _) => src.EmployeeId.HasValue))
                .ForMember(dest => dest.WitnessName, opt => opt.Condition((src, _, _) => src.WitnessName is not null))
                .ForMember(dest => dest.WitnessContact, opt => opt.Condition((src, _, _) => src.WitnessContact is not null))
                .ForMember(dest => dest.Statement, opt => opt.Condition((src, _, _) => src.Statement is not null));
        }
    }
}
