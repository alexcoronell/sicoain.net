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
                .WithMessage("El formato del correo electrónico no es válido.");

            RuleFor(x => x.FullName)
                .Length(2, 100).When(x => !string.IsNullOrWhiteSpace(x.FullName))
                .WithMessage("El nombre debe tener entre 2 y 100 caracteres.");

            // IsActive is a nullable boolean; no validation needed besides being a valid bool
        }
    }
}
