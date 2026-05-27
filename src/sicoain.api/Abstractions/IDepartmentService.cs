using sicoain.shared.DTOs.Departments;

namespace sicoain.api.Abstractions
{
    public interface IDepartmentService : IBaseService<DepartmentDto, CreateDepartmentRequest, UpdateDepartmentRequest>
    {

    }
}
