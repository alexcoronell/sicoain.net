using FluentValidation;
using sicoain.shared.DTOs.Employees;

namespace sicoain.api.Validators.Employees
{
    public class UpdateEmployeePhoneRequestValidator : AbstractValidator<UpdateEmployeePhoneRequest>
    {
        public UpdateEmployeePhoneRequestValidator()
        {
            RuleFor(x => x.Phone)
                .Matches(@"^(\+?57)?[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Invalid Colombian phone number. Use 10 digits or include +57.");
        }
    }
}
