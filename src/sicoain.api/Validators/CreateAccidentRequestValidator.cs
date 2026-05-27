using FluentValidation;
using sicoain.shared.DTOs.Accident;

namespace sicoain.api.Validators
{
    public class CreateAccidentRequestValidator : AbstractValidator<CreateAccidentRequest>
    {
        public CreateAccidentRequestValidator()
        {
            RuleFor(x => x.EventDate)
                .NotEmpty().WithMessage("Event date is required.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Event date cannot be in the future.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(10, 500).WithMessage("Description must be between 10 and 500 characters.");

            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId must be a valid identifier.");

            RuleFor(x => x.AccidentTypeId)
                .GreaterThan(0).WithMessage("AccidentTypeId must be a valid identifier.");

            RuleFor(x => x.EventCategoryId)
                .GreaterThan(0).WithMessage("EventCategoryId must be a valid identifier.");
        }
    }
}
