using FluentValidation;
using sicoain.shared.DTOs.Branches;

namespace sicoain.api.Validators
{
    public class UpdateBranchEmailRequestValidator : AbstractValidator<UpdateBranchEmailRequest>
    {
        public UpdateBranchEmailRequestValidator()
        {
            // Asumiendo que UpdateBranchEmailRequest hereda de UpdateEntityEmailRequest con Email (nullable)
            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.");
        }
    }
}
