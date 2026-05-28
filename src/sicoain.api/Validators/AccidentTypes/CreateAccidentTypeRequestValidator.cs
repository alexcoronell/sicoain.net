using FluentValidation;
using sicoain.shared.DTOs.AccidentTypes;

namespace sicoain.api.Validators.AccidentTypes
{
    public class CreateAccidentTypeRequestValidator : AbstractValidator<CreateAccidentTypeRequest>
    {
        public CreateAccidentTypeRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Accident type name is required.")
                .Length(3, 100).WithMessage("Accident type name must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");

            RuleFor(x => x.Severity)
                .IsInEnum().WithMessage("Invalid severity value.");
        }
    }
}
