using FluentValidation;
using sicoain.shared.DTOs.Business;

namespace sicoain.api.Validators.Businesses
{
    public class CreateBusinessRequestValidator : AbstractValidator<CreateBusinessRequest>
    {
        public CreateBusinessRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Business name is required.")
                .Length(3, 100).WithMessage("Business name must be between 3 and 100 characters.");

            RuleFor(x => x.AddressStreet)
                .MaximumLength(200).WithMessage("Address street cannot exceed 200 characters.");
        }
    }
}
