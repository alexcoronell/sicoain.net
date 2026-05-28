using FluentValidation;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.api.Validators.OccupationalRiskAdministrators
{
    public class UpdateOccupationalRiskAdministratorRequestValidator : AbstractValidator<UpdateOccupationalRiskAdministratorRequest>
    {
        public UpdateOccupationalRiskAdministratorRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("ORA name must be between 3 and 100 characters.");

            RuleFor(x => x.AddressStreet)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.AddressStreet))
                .WithMessage("Address street cannot exceed 200 characters.");
        }
    }
}
