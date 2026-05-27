using FluentValidation;
using sicoain.shared.DTOs.HealthPromotionEntities;

namespace sicoain.api.Validators
{
    public class CreateHealthPromotionEntityEmailRequestValidator : BaseCreateEntityEmailRequestValidator<CreateHealthPromotionEntityEmailRequest>
    {
        public CreateHealthPromotionEntityEmailRequestValidator()
        {
            RuleFor(x => x.HealthPromotionEntityId)
                .GreaterThan(0).WithMessage("HealthPromotionEntityId must be a valid identifier.");
        }
    }
}
