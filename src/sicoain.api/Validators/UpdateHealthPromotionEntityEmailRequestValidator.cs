using FluentValidation;
using sicoain.shared.DTOs.HealthPromotionEntities;

namespace sicoain.api.Validators
{
    public class UpdateHealthPromotionEntityEmailRequestValidator : AbstractValidator<UpdateHealthPromotionEntityEmailRequest>
    {
        public UpdateHealthPromotionEntityEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.");
        }
    }
}
