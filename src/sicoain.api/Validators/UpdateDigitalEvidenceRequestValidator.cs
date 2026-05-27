using FluentValidation;
using sicoain.shared.DTOs.DigitalEvidences;

namespace sicoain.api.Validators
{
    public class UpdateDigitalEvidenceRequestValidator : AbstractValidator<UpdateDigitalEvidenceRequest>
    {
        public UpdateDigitalEvidenceRequestValidator()
        {
            RuleFor(x => x.FileName)
                .MaximumLength(255).When(x => !string.IsNullOrWhiteSpace(x.FileName))
                .WithMessage("File name cannot exceed 255 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.TakenAt)
                .LessThanOrEqualTo(DateTime.UtcNow).When(x => x.TakenAt.HasValue)
                .WithMessage("Taken date cannot be in the future.");

            RuleFor(x => x.TakenByName)
                .MaximumLength(150).When(x => !string.IsNullOrWhiteSpace(x.TakenByName))
                .WithMessage("Taken by name cannot exceed 150 characters.");

            RuleFor(x => x.ChainOfCustody)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.ChainOfCustody))
                .WithMessage("Chain of custody cannot exceed 500 characters.");
        }
    }
}
