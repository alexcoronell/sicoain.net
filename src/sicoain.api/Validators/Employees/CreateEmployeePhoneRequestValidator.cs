using FluentValidation;
using sicoain.shared.DTOs.Employees;

namespace sicoain.api.Validators.Employees
{
    public class CreateEmployeePhoneRequestValidator : BaseCreateEntityPhoneRequestValidator<CreateEmployeePhoneRequest>
    {
        public CreateEmployeePhoneRequestValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId must be a valid identifier.");
        }
    }
}
