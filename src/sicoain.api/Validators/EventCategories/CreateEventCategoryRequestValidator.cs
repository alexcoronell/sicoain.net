using FluentValidation;
using sicoain.shared.DTOs.EventCategories;

namespace sicoain.api.Validators.EventCategories
{
    public class CreateEventCategoryRequestValidator : AbstractValidator<CreateEventCategoryRequest>
    {
        public CreateEventCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Event category name is required.")
                .Length(3, 100).WithMessage("Event category name must be between 3 and 100 characters.");

            RuleFor(x => x.LevelOfSeverity)
                .IsInEnum().WithMessage("Invalid severity level.");
        }
    }
}
