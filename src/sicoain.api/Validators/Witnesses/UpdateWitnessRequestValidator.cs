using FluentValidation;
using sicoain.shared.DTOs.Witnesses;

namespace sicoain.api.Validators
{
    public class UpdateWitnessRequestValidator : AbstractValidator<UpdateWitnessRequest>
    {
        public UpdateWitnessRequestValidator()
        {
            // AccidentId: if provided, must be > 0
            RuleFor(x => x.AccidentId)
                .GreaterThan(0).When(x => x.AccidentId.HasValue)
                .WithMessage("AccidentId must be a valid identifier.");

            // If EmployeeId is provided, it must be > 0 and WitnessName should be null (avoid confusion)
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).When(x => x.EmployeeId.HasValue)
                .WithMessage("EmployeeId must be > 0 if provided.");

            // If both EmployeeId and WitnessName are provided, it's invalid (cannot be both)
            RuleFor(x => x)
                .Must(x => !(x.EmployeeId.HasValue && !string.IsNullOrWhiteSpace(x.WitnessName)))
                .WithMessage("Cannot provide both EmployeeId and WitnessName at the same time.");

            // WitnessName: optional, max 150
            RuleFor(x => x.WitnessName)
                .MaximumLength(150).WithMessage("Witness name cannot exceed 150 characters.");

            // WitnessContact: optional, max 100
            RuleFor(x => x.WitnessContact)
                .MaximumLength(100).WithMessage("Witness contact cannot exceed 100 characters.");

            // Statement: optional, but if provided must be between 50 and 500
            RuleFor(x => x.Statement)
                .Length(50, 500).When(x => !string.IsNullOrWhiteSpace(x.Statement))
                .WithMessage("Statement must be between 50 and 500 characters.");
        }
    }
}
