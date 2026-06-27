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

            When(x => x.Emails != null, () =>
            {
                RuleForEach(x => x.Emails)
                    .NotEmpty().WithMessage("Each email cannot be empty.")
                    .EmailAddress().WithMessage("Each email must be a valid email address.")
                    .MaximumLength(100).WithMessage("Each email cannot exceed 100 characters.");
            });

            When(x => x.Phones != null, () =>
            {
                RuleForEach(x => x.Phones)
                    .NotEmpty().WithMessage("Each phone cannot be empty.")
                    .Matches(@"^(\+?57)?[0-9]{10}$")
                    .WithMessage("Each phone must be a valid Colombian number (10 digits, optionally with +57).");
            });
        }
    }
}
