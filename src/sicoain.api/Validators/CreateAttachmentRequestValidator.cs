using FluentValidation;
using sicoain.shared.DTOs.Attachments;

namespace sicoain.api.Validators
{
    public class CreateAttachmentRequestValidator : AbstractValidator<CreateAttachmentRequest>
    {
        public CreateAttachmentRequestValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .MaximumLength(255).WithMessage("File name cannot exceed 255 characters.");

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("MIME type is required.")
                .MaximumLength(100).WithMessage("MIME type cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.EntityType)
                .IsInEnum().WithMessage("Invalid entity type.");

            RuleFor(x => x.EntityId)
                .GreaterThan(0).WithMessage("EntityId must be a valid identifier.");

            RuleFor(x => x.Base64Content)
                .NotEmpty().WithMessage("File content is required.")
                .Must(content => IsValidBase64(content))
                .WithMessage("Invalid Base64 content.");
        }

        private bool IsValidBase64(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;
            try
            {
                Convert.FromBase64String(content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
