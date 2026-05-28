using FluentValidation;
using sicoain.shared.DTOs.Positions;

namespace sicoain.api.Validators.Positions
{
    public class CreatePositionRequestValidator : AbstractValidator<CreatePositionRequest>
    {
        public CreatePositionRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Position name is required.")
                .Length(3, 100).WithMessage("Position name must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("DepartmentId must be a valid identifier.");

            RuleFor(x => x.RiskClassId)
                .GreaterThan(0).WithMessage("RiskClassId must be a valid identifier.");
        }
    }
}
