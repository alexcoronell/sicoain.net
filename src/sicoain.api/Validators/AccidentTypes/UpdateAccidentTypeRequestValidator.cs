using FluentValidation;
using sicoain.shared.DTOs.AccidentTypes;
using sicoain.shared.Enums;

namespace sicoain.api.Validators.AccidentTypes
{
    public class UpdateAccidentTypeRequestValidator : AbstractValidator<UpdateAccidentTypeRequest>
    {
        public UpdateAccidentTypeRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Accident type name must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(250).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description cannot exceed 250 characters.");

            // Severity is non-nullable, so it will always be sent if the field is present in JSON.
            // If the client omits it, model binding will set default value (0), which is invalid.
            // To allow omission, the property should be made nullable. If you keep non-nullable,
            // use IsInEnum without When.
            RuleFor(x => x.Severity)
                .IsInEnum().WithMessage("Invalid severity value.");
        }
    }
}
