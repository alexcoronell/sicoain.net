using AutoMapper;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.Departments;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class DepartmentService : BaseService<Department, DepartmentDto, CreateDepartmentRequest, UpdateDepartmentRequest>, IDepartmentService
    {
        public DepartmentService(ApplicationDbContext context, IMapper _mapper) : base(context, _mapper)
        {
        }
    }
}
