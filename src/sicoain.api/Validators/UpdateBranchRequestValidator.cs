using FluentValidation;
using sicoain.shared.DTOs.Branches;

namespace sicoain.api.Validators
{
    public class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequest>
    {
        public UpdateBranchRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Branch name must be between 3 and 100 characters.");

            RuleFor(x => x.AddressStreet)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.AddressStreet))
                .WithMessage("Address street cannot exceed 200 characters.");

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).When(x => x.BusinessId.HasValue)
                .WithMessage("BusinessId must be a valid identifier.");
        }
    }
}
