using FluentValidation;
using sicoain.shared.DTOs.Users;

namespace sicoain.api.Validators.Users
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Must(p => !string.IsNullOrEmpty(p) && p.Any(char.IsUpper)).WithMessage("La contraseña debe contener al menos una mayúscula.")
                .Must(p => !string.IsNullOrEmpty(p) && p.Any(char.IsLower)).WithMessage("La contraseña debe contener al menos una minúscula.")
                .Must(p => !string.IsNullOrEmpty(p) && p.Any(char.IsDigit)).WithMessage("La contraseña debe contener al menos un dígito.")
                .Must(p => !string.IsNullOrEmpty(p) && p.Any(ch => !char.IsLetterOrDigit(ch))).WithMessage("La contraseña debe contener al menos un carácter especial (ej. !@#$%).");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("El nombre completo es obligatorio.")
                .Length(2, 100).WithMessage("El nombre debe tener entre 2 y 100 caracteres.");

            // Roles is optional, no validation needed (list can be empty)
        }
    }
}
