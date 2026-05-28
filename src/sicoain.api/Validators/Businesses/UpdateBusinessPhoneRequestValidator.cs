using FluentValidation;
using sicoain.shared.DTOs.Business;

namespace sicoain.api.Validators.Businesses
{
    public class UpdateBusinessPhoneRequestValidator : AbstractValidator<UpdateBusinessPhoneRequest>
    {
        public UpdateBusinessPhoneRequestValidator()
        {
            RuleFor(x => x.Phone)
                .Matches(@"^(\+?57)?[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Invalid Colombian phone number. Use 10 digits or include +57.");
        }
    }
}
