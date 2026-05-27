using FluentValidation;
using sicoain.shared.DTOs.Business;

namespace sicoain.api.Validators
{
    public class UpdateBusinessEmailRequestValidator : AbstractValidator<UpdateBusinessEmailRequest>
    {
        public UpdateBusinessEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.");
        }
    }
}
