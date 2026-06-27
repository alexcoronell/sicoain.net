using FluentValidation;
using sicoain.shared.DTOs.Users;

namespace sicoain.api.Validators.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La nueva contraseña debe tener al menos 8 caracteres.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("La confirmación de contraseña es obligatoria.")
                .Equal(x => x.NewPassword).WithMessage("La nueva contraseña y la confirmación no coinciden.");
        }
    }
}
