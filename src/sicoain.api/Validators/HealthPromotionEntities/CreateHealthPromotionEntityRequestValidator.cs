using FluentValidation;
using sicoain.shared.DTOs.HealthPromotionEntities;

namespace sicoain.api.Validators.HealthPromotionEntities
{
    public class CreateHealthPromotionEntityRequestValidator : AbstractValidator<CreateHealthPromotionEntityRequest>
    {
        public CreateHealthPromotionEntityRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("EPS name is required.")
                .Length(3, 100).WithMessage("HPE name must be between 3 and 100 characters.");

            RuleFor(x => x.AddressStreet)
                .MaximumLength(200).WithMessage("Address street cannot exceed 200 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
