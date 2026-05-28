using FluentValidation;
using sicoain.shared.DTOs.DigitalEvidences;

namespace sicoain.api.Validators.DigitalEvidences
{
    public class CreateDigitalEvidenceRequestValidator : AbstractValidator<CreateDigitalEvidenceRequest>
    {
        public CreateDigitalEvidenceRequestValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .MaximumLength(255).WithMessage("File name cannot exceed 255 characters.");

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("MIME type is required.")
                .MaximumLength(100).WithMessage("MIME type cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.TakenAt)
                .NotEmpty().WithMessage("Taken date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Taken date cannot be in the future.");

            RuleFor(x => x.TakenByName)
                .MaximumLength(150).WithMessage("Taken by name cannot exceed 150 characters.");

            RuleFor(x => x.ChainOfCustody)
                .MaximumLength(500).WithMessage("Chain of custody cannot exceed 500 characters.");

            RuleFor(x => x.AccidentId)
                .GreaterThan(0).When(x => x.AccidentId.HasValue)
                .WithMessage("AccidentId must be a valid identifier.");

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
