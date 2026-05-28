using FluentValidation;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.api.Validators.OccupationalRiskAdministrators
{
    public class UpdateOccupationalRiskAdministratorEmailRequestValidator : AbstractValidator<UpdateOccupationalRiskAdministratorEmailRequest>
    {
        public UpdateOccupationalRiskAdministratorEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.");
        }
    }
}
