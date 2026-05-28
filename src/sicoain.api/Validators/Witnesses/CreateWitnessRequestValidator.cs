using FluentValidation;
using sicoain.shared.DTOs.Witnesses;

namespace sicoain.api.Validators
{
    public class CreateWitnessRequestValidator : AbstractValidator<CreateWitnessRequest>
    {
        public CreateWitnessRequestValidator()
        {
            // AccidentId must be > 0
            RuleFor(x => x.AccidentId)
                .GreaterThan(0).WithMessage("AccidentId must be a valid identifier.");

            // Either EmployeeId or WitnessName must be provided (mutually exclusive)
            RuleFor(x => x)
                .Must(x => (x.EmployeeId.HasValue && x.EmployeeId > 0 && string.IsNullOrWhiteSpace(x.WitnessName))
                       || (!x.EmployeeId.HasValue && !string.IsNullOrWhiteSpace(x.WitnessName)))
                .WithMessage("Either a valid EmployeeId or a WitnessName is required, but not both.");

            // If EmployeeId is provided, it must be > 0 (already covered by the Must rule, but explicit)
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).When(x => x.EmployeeId.HasValue)
                .WithMessage("EmployeeId must be > 0 if provided.");

            // WitnessName: if provided, max length 150
            RuleFor(x => x.WitnessName)
                .MaximumLength(150).WithMessage("Witness name cannot exceed 150 characters.");

            // WitnessContact: optional, max 100
            RuleFor(x => x.WitnessContact)
                .MaximumLength(100).WithMessage("Witness contact cannot exceed 100 characters.");

            // Statement: required, length between 50 and 500
            RuleFor(x => x.Statement)
                .NotEmpty().WithMessage("Statement is required.")
                .Length(50, 500).WithMessage("Statement must be between 50 and 500 characters.");
        }
    }
}
