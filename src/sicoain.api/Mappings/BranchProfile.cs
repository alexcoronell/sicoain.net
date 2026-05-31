using AutoMapper;
using sicoain.shared.DTOs.Branches;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class BranchProfile : Profile
    {
        public BranchProfile()
        {
            // ========================================================================
            // Branch (entity) -> BranchDto
            // ========================================================================
            CreateMap<Branch, BranchDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(
                    src => src.Business != null ? src.Business.Name : null));

            // ========================================================================
            // CreateBranchRequest -> Branch (entity)
            // ========================================================================
            CreateMap<CreateBranchRequest, Branch>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Business, opt => opt.Ignore())
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
            // UpdateBranchRequest -> Branch (entity)
            // ========================================================================
            CreateMap<UpdateBranchRequest, Branch>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Business, opt => opt.Ignore())
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
            // BranchPhone (entity) -> BranchPhoneDto
            // ========================================================================
            CreateMap<BranchPhone, BranchPhoneDto>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone));

            // ========================================================================
            // CreateBranchPhoneRequest -> BranchPhone (entity)
            // ========================================================================
            CreateMap<CreateBranchPhoneRequest, BranchPhone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.PhoneType, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore());

            // ========================================================================
            // UpdateBranchPhoneRequest -> BranchPhone (entity)
            // ========================================================================
            CreateMap<UpdateBranchPhoneRequest, BranchPhone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneType, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========================================================================
            // BranchEmail (entity) -> BranchEmailDto
            // ========================================================================
            CreateMap<BranchEmail, BranchEmailDto>();

            // ========================================================================
            // CreateBranchEmailRequest -> BranchEmail (entity)
            // ========================================================================
            CreateMap<CreateBranchEmailRequest, BranchEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.Branch, opt => opt.Ignore());

            // ========================================================================
            // UpdateBranchEmailRequest -> BranchEmail (entity)
            // ========================================================================
            CreateMap<UpdateBranchEmailRequest, BranchEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
