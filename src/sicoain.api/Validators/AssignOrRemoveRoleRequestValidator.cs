using FluentValidation;
using sicoain.shared.DTOs.Users;

namespace sicoain.api.Validators
{
    public class AssignOrRemoveRoleRequestValidator : AbstractValidator<AssignOrRemoveRoleRequest>
    {
        public AssignOrRemoveRoleRequestValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.");
        }
    }
}
