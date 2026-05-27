using FluentValidation;
using sicoain.shared.DTOs.Positions;

namespace sicoain.api.Validators
{
    public class UpdatePositionRequestValidator : AbstractValidator<UpdatePositionRequest>
    {
        public UpdatePositionRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Position name must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).When(x => x.DepartmentId.HasValue)
                .WithMessage("DepartmentId must be a valid identifier.");

            RuleFor(x => x.RiskClassId)
                .GreaterThan(0).When(x => x.RiskClassId.HasValue)
                .WithMessage("RiskClassId must be a valid identifier.");
        }
    }
}
