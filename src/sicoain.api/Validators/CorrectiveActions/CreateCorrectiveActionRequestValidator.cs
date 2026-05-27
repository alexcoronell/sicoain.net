using FluentValidation;
using sicoain.shared.DTOs.CorrectiveActions;

namespace sicoain.api.Validators.CorrectiveActions
{
    public class CreateCorrectiveActionRequestValidator : AbstractValidator<CreateCorrectiveActionRequest>
    {
        public CreateCorrectiveActionRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(3, 500).WithMessage("Description must be between 3 and 500 characters.");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Due date cannot be in the past.");

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Invalid status value.");

            RuleFor(x => x.Priority)
                .IsInEnum().When(x => x.Priority.HasValue)
                .WithMessage("Invalid priority value.");

            RuleFor(x => x.AccidentId)
                .GreaterThan(0).WithMessage("AccidentId must be a valid identifier.");
        }
    }
}
