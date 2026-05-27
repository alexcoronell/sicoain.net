using FluentValidation;
using sicoain.shared.DTOs.Employees;

namespace sicoain.api.Validators
{
    public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
    {
        public CreateEmployeeRequestValidator()
        {
            // DocumentType: must be a valid enum value
            RuleFor(x => x.DocumentType)
                .IsInEnum()
                .WithMessage("Invalid document type.");

            // DocumentNumber: required, length between 5 and 20 characters
            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("Document number is required.")
                .Length(5, 20).WithMessage("Document number must be between 5 and 20 characters.");

            // FirstName: required, length between 2 and 50 characters
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters.");

            // SecondName: optional, max 50 characters
            RuleFor(x => x.SecondName)
                .MaximumLength(50).WithMessage("Second name cannot exceed 50 characters.");

            // Surname: required, length between 2 and 50 characters
            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .Length(2, 50).WithMessage("Surname must be between 2 and 50 characters.");

            // SecondSurname: optional, max 50 characters
            RuleFor(x => x.SecondSurname)
                .MaximumLength(50).WithMessage("Second surname cannot exceed 50 characters.");

            // State: required, max 100 characters
            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

            // Municipality: required, max 100 characters
            RuleFor(x => x.Municipality)
                .NotEmpty().WithMessage("Municipality is required.")
                .MaximumLength(100).WithMessage("Municipality cannot exceed 100 characters.");

            // Neighborhood: required, max 100 characters
            RuleFor(x => x.Neighborhood)
                .NotEmpty().WithMessage("Neighborhood is required.")
                .MaximumLength(100).WithMessage("Neighborhood cannot exceed 100 characters.");

            // AddressStreet: required, length between 5 and 200 characters
            RuleFor(x => x.AddressStreet)
                .NotEmpty().WithMessage("Address is required.")
                .Length(5, 200).WithMessage("Address must be between 5 and 200 characters.");

            // AlternativeAddressStreet: optional, max 200 characters
            RuleFor(x => x.AlternativeAddressStreet)
                .MaximumLength(200).WithMessage("Alternative address cannot exceed 200 characters.");

            // PostalCode: optional, max 20 characters
            RuleFor(x => x.PostalCode)
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters.");

            // HiringDate: required, cannot be in the future
            RuleFor(x => x.HiringDate)
                .NotEmpty().WithMessage("Hiring date is required.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Hiring date cannot be in the future.");

            // TerminationDate: optional, if provided must be >= HiringDate
            RuleFor(x => x.TerminationDate)
                .GreaterThanOrEqualTo(x => x.HiringDate)
                .When(x => x.TerminationDate.HasValue)
                .WithMessage("Termination date must be on or after hiring date.");

            // Diseases, Medications, Allergies, Notes: optional, max 500 characters
            RuleFor(x => x.Diseases).MaximumLength(500).WithMessage("Diseases cannot exceed 500 characters.");
            RuleFor(x => x.Medications).MaximumLength(500).WithMessage("Medications cannot exceed 500 characters.");
            RuleFor(x => x.Allergies).MaximumLength(500).WithMessage("Allergies cannot exceed 500 characters.");
            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

            // Foreign keys: must be positive integers
            RuleFor(x => x.BusinessId).GreaterThan(0).WithMessage("BusinessId must be a valid identifier.");
            RuleFor(x => x.BranchId).GreaterThan(0).WithMessage("BranchId must be a valid identifier.");
            RuleFor(x => x.HealthPromotionEntityId).GreaterThan(0).WithMessage("HealthPromotionEntityId must be a valid identifier.");
            RuleFor(x => x.OccupationalRiskAdministratorId).GreaterThan(0).WithMessage("OccupationalRiskAdministratorId must be a valid identifier.");
            RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("DepartmentId must be a valid identifier.");
            RuleFor(x => x.PositionId).GreaterThan(0).WithMessage("PositionId must be a valid identifier.");
        }
    }
}
