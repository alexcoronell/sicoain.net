using FluentValidation;
using sicoain.shared.DTOs;

namespace sicoain.api.Validators
{
    public abstract class BaseCreateEntityPhoneRequestValidator<T> : AbstractValidator<T> where T : CreateEntityPhoneRequest
    {
        protected BaseCreateEntityPhoneRequestValidator()
        {
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(\+?57)?[0-9]{10}$").WithMessage("Invalid Colombian phone number. Use 10 digits or include +57.");
        }
    }
}
