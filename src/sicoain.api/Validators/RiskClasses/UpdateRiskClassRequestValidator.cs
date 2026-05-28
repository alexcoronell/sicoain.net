using FluentValidation;
using sicoain.shared.DTOs.RiskClasses;

namespace sicoain.api.Validators.RiskClasses
{
    public class UpdateRiskClassRequestValidator : AbstractValidator<UpdateRiskClassRequest>
    {
        public UpdateRiskClassRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Risk class name must be between 3 and 100 characters.");

            RuleFor(x => x.Code)
                .Length(1, 5).When(x => !string.IsNullOrWhiteSpace(x.Code))
                .WithMessage("Code must be between 1 and 5 characters.");

            RuleFor(x => x.ContributionRate)
                .InclusiveBetween(0.0001m, 9.9999m)
                .When(x => x.ContributionRate.HasValue)
                .WithMessage("Contribution rate must be between 0.0001 and 9.9999.");

            RuleFor(x => x.IsActive)
                .NotNull().When(x => x.IsActive.HasValue) // just to ensure it's a valid boolean if provided
                .WithMessage("IsActive must be true or false.");
        }
    }
}
