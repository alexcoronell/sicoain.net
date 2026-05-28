using FluentValidation;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.api.Validators.OccupationalRiskAdministrators
{
    public class UpdateOccupationalRiskAdministratorPhoneRequestValidator : AbstractValidator<UpdateOccupationalRiskAdministratorPhoneRequest>
    {
        public UpdateOccupationalRiskAdministratorPhoneRequestValidator()
        {
            RuleFor(x => x.Phone)
                .Matches(@"^(\+?57)?[0-9]{10}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Invalid Colombian phone number. Use 10 digits or include +57.");
        }
    }
}
