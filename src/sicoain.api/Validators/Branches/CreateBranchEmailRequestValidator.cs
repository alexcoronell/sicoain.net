using FluentValidation;
using sicoain.shared.DTOs.Branches;

namespace sicoain.api.Validators
{
    public class CreateBranchEmailRequestValidator : BaseCreateEntityEmailRequestValidator<CreateBranchEmailRequest>
    {
        public CreateBranchEmailRequestValidator()
        {
            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("BranchId must be a valid identifier.");
        }
    }
}
