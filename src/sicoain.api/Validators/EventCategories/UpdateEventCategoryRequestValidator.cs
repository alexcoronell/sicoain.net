using FluentValidation;
using sicoain.shared.DTOs.EventCategories;

namespace sicoain.api.Validators.EventCategories
{
    public class UpdateEventCategoryRequestValidator : AbstractValidator<UpdateEventCategoryRequest>
    {
        public UpdateEventCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .Length(3, 100).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("Event category name must be between 3 and 100 characters.");

            RuleFor(x => x.LevelOfSeverity)
                .IsInEnum().When(x => x.LevelOfSeverity.HasValue)
                .WithMessage("Invalid severity level.");
        }
    }
}
