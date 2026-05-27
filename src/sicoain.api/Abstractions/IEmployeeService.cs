using sicoain.shared.DTOs.Employees;

namespace sicoain.api.Abstractions
{
    public interface IEmployeeService : IBaseService<EmployeeDto, CreateEmployeeRequest, UpdateEmployeeRequest>
    {

    }
}
