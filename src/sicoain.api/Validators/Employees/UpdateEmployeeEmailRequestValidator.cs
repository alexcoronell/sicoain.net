using FluentValidation;
using sicoain.shared.DTOs.Employees;

namespace sicoain.api.Validators.Employees
{
    public class UpdateEmployeeEmailRequestValidator : AbstractValidator<UpdateEmployeeEmailRequest>
    {
        public UpdateEmployeeEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.");
        }
    }
}
