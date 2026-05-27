using FluentValidation;
using sicoain.shared.DTOs.Business;

namespace sicoain.api.Validators
{
    public class CreateBusinessPhoneRequestValidator : BaseCreateEntityPhoneRequestValidator<CreateBusinessPhoneRequest>
    {
        public CreateBusinessPhoneRequestValidator()
        {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("BusinessId must be a valid identifier.");
        }
    }
}
