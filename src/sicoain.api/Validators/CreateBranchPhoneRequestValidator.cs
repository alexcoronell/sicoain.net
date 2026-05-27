using FluentValidation;
using sicoain.shared.DTOs.Branches;

namespace sicoain.api.Validators
{
    public class CreateBranchPhoneRequestValidator : BaseCreateEntityPhoneRequestValidator<CreateBranchPhoneRequest>
    {
        public CreateBranchPhoneRequestValidator()
        {
            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("BranchId must be a valid identifier.");
        }
    }
}
