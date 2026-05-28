using FluentValidation;
using sicoain.shared.DTOs.Branches;

namespace sicoain.api.Validators
{
    public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
    {
        public CreateBranchRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Branch name is required.")
                .Length(3, 100).WithMessage("Branch name must be between 3 and 100 characters.");

            RuleFor(x => x.AddressStreet)
                .MaximumLength(200).WithMessage("Address street cannot exceed 200 characters.");

            RuleFor(x => x.BusinessId)
                .NotNull().WithMessage("BusinessId is required.")
                .GreaterThan(0).WithMessage("BusinessId must be a valid identifier.");
        }
    }
}
