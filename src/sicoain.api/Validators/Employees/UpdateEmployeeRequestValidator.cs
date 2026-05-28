using FluentValidation;
using sicoain.shared.DTOs.Employees;

namespace sicoain.api.Validators
{
    public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
    {
        public UpdateEmployeeRequestValidator()
        {
            // DocumentType: if provided, must be a valid enum value
            RuleFor(x => x.DocumentType)
                .IsInEnum()
                .When(x => x.DocumentType.HasValue)
                .WithMessage("Invalid document type.");

            // DocumentNumber: if provided, length between 5 and 20
            RuleFor(x => x.DocumentNumber)
                .Length(5, 20)
                .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber))
                .WithMessage("Document number must be between 5 and 20 characters.");

            // FirstName: if provided, length between 2 and 50
            RuleFor(x => x.FirstName)
                .Length(2, 50)
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName))
                .WithMessage("First name must be between 2 and 50 characters.");

            // SecondName: if provided, max 50 characters
            RuleFor(x => x.SecondName)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.SecondName))
                .WithMessage("Second name cannot exceed 50 characters.");

            // Surname: if provided, length between 2 and 50
            RuleFor(x => x.Surname)
                .Length(2, 50)
                .When(x => !string.IsNullOrWhiteSpace(x.Surname))
                .WithMessage("Surname must be between 2 and 50 characters.");

            // SecondSurname: if provided, max 50 characters
            RuleFor(x => x.SecondSurname)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.SecondSurname))
                .WithMessage("Second surname cannot exceed 50 characters.");

            // State: if provided, max 100 characters
            RuleFor(x => x.State)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.State))
                .WithMessage("State cannot exceed 100 characters.");

            // Municipality: if provided, max 100 characters
            RuleFor(x => x.Municipality)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Municipality))
                .WithMessage("Municipality cannot exceed 100 characters.");

            // Neighborhood: if provided, max 100 characters
            RuleFor(x => x.Neighborhood)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Neighborhood))
                .WithMessage("Neighborhood cannot exceed 100 characters.");

            // AddressStreet: if provided, length between 5 and 200
            RuleFor(x => x.AddressStreet)
                .Length(5, 200)
                .When(x => !string.IsNullOrWhiteSpace(x.AddressStreet))
                .WithMessage("Address must be between 5 and 200 characters.");

            // AlternativeAddressStreet: if provided, max 200 characters
            RuleFor(x => x.AlternativeAddressStreet)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.AlternativeAddressStreet))
                .WithMessage("Alternative address cannot exceed 200 characters.");

            // PostalCode: if provided, max 20 characters
            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.PostalCode))
                .WithMessage("Postal code cannot exceed 20 characters.");

            // HiringDate: if provided, cannot be in the future
            RuleFor(x => x.HiringDate)
                .LessThanOrEqualTo(DateTime.Today)
                .When(x => x.HiringDate.HasValue)
                .WithMessage("Hiring date cannot be in the future.");

            // TerminationDate: if provided, must be >= HiringDate (if HiringDate also provided)
            RuleFor(x => x.TerminationDate)
                .GreaterThanOrEqualTo(x => x.HiringDate)
                .When(x => x.TerminationDate.HasValue && x.HiringDate.HasValue)
                .WithMessage("Termination date must be on or after hiring date.");

            // Diseases: if provided, max 500 characters
            RuleFor(x => x.Diseases)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Diseases))
                .WithMessage("Diseases cannot exceed 500 characters.");

            // Medications: if provided, max 500 characters
            RuleFor(x => x.Medications)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Medications))
                .WithMessage("Medications cannot exceed 500 characters.");

            // Allergies: if provided, max 500 characters
            RuleFor(x => x.Allergies)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Allergies))
                .WithMessage("Allergies cannot exceed 500 characters.");

            // Notes: if provided, max 500 characters
            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Notes))
                .WithMessage("Notes cannot exceed 500 characters.");

            // Foreign keys: if provided, must be > 0
            RuleFor(x => x.BusinessId)
                .GreaterThan(0)
                .When(x => x.BusinessId.HasValue)
                .WithMessage("BusinessId must be a valid identifier.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0)
                .When(x => x.BranchId.HasValue)
                .WithMessage("BranchId must be a valid identifier.");

            RuleFor(x => x.HealthPromotionEntityId)
                .GreaterThan(0)
                .When(x => x.HealthPromotionEntityId.HasValue)
                .WithMessage("HealthPromotionEntityId must be a valid identifier.");

            RuleFor(x => x.OccupationalRiskAdministratorId)
                .GreaterThan(0)
                .When(x => x.OccupationalRiskAdministratorId.HasValue)
                .WithMessage("OccupationalRiskAdministratorId must be a valid identifier.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .When(x => x.DepartmentId.HasValue)
                .WithMessage("DepartmentId must be a valid identifier.");

            RuleFor(x => x.PositionId)
                .GreaterThan(0)
                .When(x => x.PositionId.HasValue)
                .WithMessage("PositionId must be a valid identifier.");
        }
    }
}
