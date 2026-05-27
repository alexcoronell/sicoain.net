using FluentValidation;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.api.Validators
{
    public class CreateOccupationalRiskAdministratorPhoneRequestValidator : BaseCreateEntityPhoneRequestValidator<CreateOccupationalRiskAdministratorPhoneRequest>
    {
        public CreateOccupationalRiskAdministratorPhoneRequestValidator()
        {
            RuleFor(x => x.OccupationalRiskAdministratorId)
                .GreaterThan(0).WithMessage("OccupationalRiskAdministratorId must be a valid identifier.");
        }
    }
}
