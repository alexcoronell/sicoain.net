using FluentValidation;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.api.Validators
{
    public class CreateOccupationalRiskAdministratorEmailRequestValidator : BaseCreateEntityEmailRequestValidator<CreateOccupationalRiskAdministratorEmailRequest>
    {
        public CreateOccupationalRiskAdministratorEmailRequestValidator()
        {
            RuleFor(x => x.OccupationalRiskAdministratorId)
                .GreaterThan(0).WithMessage("OccupationalRiskAdministratorId must be a valid identifier.");
        }
    }
}
