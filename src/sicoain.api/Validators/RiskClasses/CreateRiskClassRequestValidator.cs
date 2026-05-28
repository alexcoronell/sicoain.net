using FluentValidation;
using sicoain.shared.DTOs.RiskClasses;

namespace sicoain.api.Validators.RiskClasses
{
    public class CreateRiskClassRequestValidator : AbstractValidator<CreateRiskClassRequest>
    {
        public CreateRiskClassRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Risk class name is required.")
                .Length(3, 100).WithMessage("Risk class name must be between 3 and 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Risk class code is required.")
                .Length(1, 5).WithMessage("Code must be between 1 and 5 characters.");

            RuleFor(x => x.ContributionRate)
                .InclusiveBetween(0.0001m, 9.9999m)
                .WithMessage("Contribution rate must be between 0.0001 and 9.9999.");
        }
    }
}
