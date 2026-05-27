using FluentValidation;
using sicoain.shared.DTOs.Departments;

namespace sicoain.api.Validators
{
    public class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
    {
        public UpdateDepartmentRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Department name must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(250).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description cannot exceed 250 characters.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Phone)
                .Matches(@"^(\+?57)?[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Invalid Colombian phone number. Use 10 digits or include +57.");
        }
    }
}
