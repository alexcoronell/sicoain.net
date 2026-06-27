using FluentValidation;
using sicoain.shared.DTOs.Users;

namespace sicoain.api.Validators.Users
{
    public class AssignOrRemoveRoleRequestValidator : AbstractValidator<AssignOrRemoveRoleRequest>
    {
        public AssignOrRemoveRoleRequestValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del rol no puede exceder 50 caracteres.");
        }
    }
}
