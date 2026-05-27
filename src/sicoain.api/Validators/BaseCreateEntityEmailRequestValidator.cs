using FluentValidation;
using sicoain.shared.DTOs;

namespace sicoain.api.Validators
{
    public abstract class BaseCreateEntityEmailRequestValidator<T> : AbstractValidator<T> where T : CreateEntityEmailRequest
    {
        protected BaseCreateEntityEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
