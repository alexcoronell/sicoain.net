using FluentValidation;
using sicoain.shared.DTOs.Accident;

namespace sicoain.api.Validators.Accidents
{
    public class UpdateAccidentRequestValidator : AbstractValidator<UpdateAccidentRequest>
    {
        public UpdateAccidentRequestValidator()
        {
            // EventDate: si se envía, no puede ser futura
            RuleFor(x => x.EventDate)
                .LessThanOrEqualTo(DateTime.Today)
                .When(x => x.EventDate.HasValue)
                .WithMessage("Event date cannot be in the future.");

            // Description: si se envía, debe tener entre 10 y 500 caracteres
            RuleFor(x => x.Description)
                .Length(10, 500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description must be between 10 and 500 characters.");

            // EmployeeId: si se envía, debe ser > 0
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .When(x => x.EmployeeId.HasValue)
                .WithMessage("EmployeeId must be a valid identifier.");

            // AccidentTypeId: si se envía, debe ser > 0
            RuleFor(x => x.AccidentTypeId)
                .GreaterThan(0)
                .When(x => x.AccidentTypeId.HasValue)
                .WithMessage("AccidentTypeId must be a valid identifier.");

            // EventCategoryId: si se envía, debe ser > 0
            RuleFor(x => x.EventCategoryId)
                .GreaterThan(0)
                .When(x => x.EventCategoryId.HasValue)
                .WithMessage("EventCategoryId must be a valid identifier.");
        }
    }
}
