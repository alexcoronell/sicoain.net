using FluentValidation;
using sicoain.shared.DTOs.Branches;

namespace sicoain.api.Validators
{
    public class UpdateBranchPhoneRequestValidator : AbstractValidator<UpdateBranchPhoneRequest>
    {
        public UpdateBranchPhoneRequestValidator()
        {
            RuleFor(x => x.Phone)
                .Matches(@"^(\+?57)?[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Invalid Colombian phone number. Use 10 digits or include +57.");
        }
    }
}
