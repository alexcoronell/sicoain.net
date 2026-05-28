using FluentValidation;
using sicoain.shared.DTOs.HealthPromotionEntities;

namespace sicoain.api.Validators.HealthPromotionEntities
{
    public class UpdateHealthPromotionEntityPhoneRequestValidator : AbstractValidator<UpdateHealthPromotionEntityPhoneRequest>
    {
        public UpdateHealthPromotionEntityPhoneRequestValidator()
        {
            RuleFor(x => x.Phone)
                .Matches(@"^(\+?57)?[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Invalid Colombian phone number. Use 10 digits or include +57.");
        }
    }
}
