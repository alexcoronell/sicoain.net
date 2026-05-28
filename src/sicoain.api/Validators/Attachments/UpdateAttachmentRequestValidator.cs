using FluentValidation;
using sicoain.shared.DTOs.Attachments;

namespace sicoain.api.Validators.Attachments
{
    public class UpdateAttachmentRequestValidator : AbstractValidator<UpdateAttachmentRequest>
    {
        public UpdateAttachmentRequestValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
