using FluentValidation;
using sicoain.shared.DTOs.Users;

namespace sicoain.api.Validators.Users
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.FullName)
                .Length(2, 100).When(x => !string.IsNullOrWhiteSpace(x.FullName))
                .WithMessage("Full name must be between 2 and 100 characters.");

            // IsActive is a nullable boolean; no validation needed besides being a valid bool
        }
    }
}
