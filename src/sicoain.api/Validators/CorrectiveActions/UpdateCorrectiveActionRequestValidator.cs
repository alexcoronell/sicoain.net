using FluentValidation;
using sicoain.shared.DTOs.CorrectiveActions;

namespace sicoain.api.Validators.CorrectiveActions
{
    public class UpdateCorrectiveActionRequestValidator : AbstractValidator<UpdateCorrectiveActionRequest>
    {
        public UpdateCorrectiveActionRequestValidator()
        {
            RuleFor(x => x.Title)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Title))
                .WithMessage("Title must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .Length(3, 500).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description must be between 3 and 500 characters.");

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Today).When(x => x.DueDate.HasValue)
                .WithMessage("Due date cannot be in the past.");

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Invalid status value.");

            RuleFor(x => x.Priority)
                .IsInEnum().When(x => x.Priority.HasValue)
                .WithMessage("Invalid priority value.");

            RuleFor(x => x.AccidentId)
                .GreaterThan(0).When(x => x.AccidentId.HasValue)
                .WithMessage("AccidentId must be a valid identifier.");
        }
    }
}
