using FluentValidation;
using sicoain.shared.DTOs.HealthPromotionEntities;

namespace sicoain.api.Validators
{
    public class UpdateHealthPromotionEntityRequestValidator : AbstractValidator<UpdateHealthPromotionEntityRequest>
    {
        public UpdateHealthPromotionEntityRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("EPS name must be between 3 and 100 characters.");

            RuleFor(x => x.AddressStreet)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.AddressStreet))
                .WithMessage("Address street cannot exceed 200 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes))
                .WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
