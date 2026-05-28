using FluentValidation;
using sicoain.shared.DTOs.HealthPromotionEntities;

namespace sicoain.api.Validators.HealthPromotionEntities
{
    public class CreateHealthPromotionEntityPhoneRequestValidator : BaseCreateEntityPhoneRequestValidator<CreateHealthPromotionEntityPhoneRequest>
    {
        public CreateHealthPromotionEntityPhoneRequestValidator()
        {
            RuleFor(x => x.HealthPromotionEntityId)
                .GreaterThan(0).WithMessage("HealthPromotionEntityId must be a valid identifier.");
        }
    }
}
