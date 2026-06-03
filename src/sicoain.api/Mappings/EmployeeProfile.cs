using AutoMapper;
using sicoain.shared.DTOs.Employees;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            // ========================================================================
            // Employee (entity) -> EmployeeDto
            // The service loads: Business, Branch, Position, Position.Department
            // HealthPromotionEntity and OccupationalRiskAdministrator are NOT loaded
            // ========================================================================
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(
                    src => src.Business != null ? src.Business.Name : null))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(
                    src => src.Branch != null ? src.Branch.Name : null))
                .ForMember(dest => dest.HealthPromotionEntityName, opt => opt.MapFrom(
                    src => src.HealthPromotionEntity != null ? src.HealthPromotionEntity.Name : null))
                .ForMember(dest => dest.OccupationalRiskAdministratorName, opt => opt.MapFrom(
                    src => src.OccupationalRiskAdministrator != null ? src.OccupationalRiskAdministrator.Name : null))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(
                    src => src.Position != null && src.Position.Department != null
                        ? src.Position.Department.Name : null))
                .ForMember(dest => dest.PositionName, opt => opt.MapFrom(
                    src => src.Position != null ? src.Position.Name : null));

            // ========================================================================
            // CreateEmployeeRequest -> Employee (entity)
            // ========================================================================
            CreateMap<CreateEmployeeRequest, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Business, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.HealthPromotionEntity, opt => opt.Ignore())
                .ForMember(dest => dest.OccupationalRiskAdministrator, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeePhones, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeEmails, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeContacts, opt => opt.Ignore())
                .ForMember(dest => dest.Witnesses, opt => opt.Ignore())
                .ForMember(dest => dest.Accidents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // ========================================================================
            // UpdateEmployeeRequest -> Employee (entity)
            // ========================================================================
            CreateMap<UpdateEmployeeRequest, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Business, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.HealthPromotionEntity, opt => opt.Ignore())
                .ForMember(dest => dest.OccupationalRiskAdministrator, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeePhones, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeEmails, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeContacts, opt => opt.Ignore())
                .ForMember(dest => dest.Witnesses, opt => opt.Ignore())
                .ForMember(dest => dest.Accidents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========================================================================
            // EmployeePhone (entity) -> EmployeePhoneDto
            // ========================================================================
            CreateMap<EmployeePhone, EmployeePhoneDto>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone));

            // ========================================================================
            // CreateEmployeePhoneRequest -> EmployeePhone (entity)
            // ========================================================================
            CreateMap<CreateEmployeePhoneRequest, EmployeePhone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.PhoneType, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore());

            // ========================================================================
            // UpdateEmployeePhoneRequest -> EmployeePhone (entity)
            // ========================================================================
            CreateMap<UpdateEmployeePhoneRequest, EmployeePhone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneType, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ========================================================================
            // EmployeeEmail (entity) -> EmployeeEmailDto
            // ========================================================================
            CreateMap<EmployeeEmail, EmployeeEmailDto>();

            // ========================================================================
            // CreateEmployeeEmailRequest -> EmployeeEmail (entity)
            // ========================================================================
            CreateMap<CreateEmployeeEmailRequest, EmployeeEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.Employee, opt => opt.Ignore());

            // ========================================================================
            // UpdateEmployeeEmailRequest -> EmployeeEmail (entity)
            // ========================================================================
            CreateMap<UpdateEmployeeEmailRequest, EmployeeEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
