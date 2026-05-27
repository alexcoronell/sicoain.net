using FluentValidation;
using sicoain.shared.DTOs.Business;

namespace sicoain.api.Validators.Businesses
{
    public class CreateBusinessEmailRequestValidator : BaseCreateEntityEmailRequestValidator<CreateBusinessEmailRequest>
    {
        public CreateBusinessEmailRequestValidator()
        {
            // Validación adicional del BusinessId
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("BusinessId must be a valid identifier.");
        }
    }
}
