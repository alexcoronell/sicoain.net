using FluentValidation;
using sicoain.shared.DTOs.Business;

namespace sicoain.api.Validators.Businesses
{
    public class UpdateBusinessRequestValidator : AbstractValidator<UpdateBusinessRequest>
    {
        public UpdateBusinessRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Business name must be between 3 and 100 characters.");

            RuleFor(x => x.AddressStreet)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.AddressStreet))
                .WithMessage("Address street cannot exceed 200 characters.");
        }
    }
}
