using FluentValidation;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;

namespace sicoain.api.Validators
{
    public class CreateOccupationalRiskAdministratorRequestValidator : AbstractValidator<CreateOccupationalRiskAdministratorRequest>
    {
        public CreateOccupationalRiskAdministratorRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("ORA name is required.")
                .Length(3, 100).WithMessage("ORA name must be between 3 and 100 characters.");

            RuleFor(x => x.AddressStreet)
                .MaximumLength(200).WithMessage("Address street cannot exceed 200 characters.");
        }
    }
}
