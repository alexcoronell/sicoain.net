using FluentValidation.TestHelper;
using sicoain.api.Validators;
using sicoain.shared.DTOs.Employees;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.Employees
{
    /// <summary>
    /// Unit tests for the UpdateEmployeeRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateEmployeeRequestValidatorTests
    {
        private readonly UpdateEmployeeRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            // Arrange
            var model = new UpdateEmployeeRequest();

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_FirstName_Is_Null_Or_Empty()
        {
            // Null and empty strings are considered "not provided" and should not trigger validation.
            var modelNull = new UpdateEmployeeRequest { FirstName = null };
            var modelEmpty = new UpdateEmployeeRequest { FirstName = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_FirstName_Is_Provided_But_Too_Short()
        {
            // Length 1 is less than minimum allowed (2)
            var model = new UpdateEmployeeRequest { FirstName = "A" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Should_Have_Error_When_FirstName_Is_Provided_But_Too_Long()
        {
            // Length 51 exceeds maximum allowed (50)
            var model = new UpdateEmployeeRequest { FirstName = new string('A', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_FirstName_Is_Provided_And_Valid()
        {
            // Valid length between 2 and 50
            var model = new UpdateEmployeeRequest { FirstName = "John" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_SecondName_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { SecondName = null };
            var modelEmpty = new UpdateEmployeeRequest { SecondName = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_SecondName_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { SecondName = new string('B', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.SecondName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Surname_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { Surname = null };
            var modelEmpty = new UpdateEmployeeRequest { Surname = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Surname_Is_Provided_But_Too_Short()
        {
            var model = new UpdateEmployeeRequest { Surname = "A" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Surname);
        }

        [Fact]
        public void Should_Have_Error_When_Surname_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { Surname = new string('C', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Surname);
        }

        [Fact]
        public void Should_Have_Error_When_DocumentNumber_Is_Provided_But_Too_Short()
        {
            var model = new UpdateEmployeeRequest { DocumentNumber = "123" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
        }

        [Fact]
        public void Should_Have_Error_When_DocumentNumber_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { DocumentNumber = new string('1', 21) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
        }

        [Fact]
        public void Should_Have_Error_When_BusinessId_Is_Provided_But_Zero()
        {
            var model = new UpdateEmployeeRequest { BusinessId = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        }

        [Fact]
        public void Should_Have_Error_When_BusinessId_Is_Provided_But_Negative()
        {
            var model = new UpdateEmployeeRequest { BusinessId = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_BusinessId_Is_Provided_And_Valid()
        {
            var model = new UpdateEmployeeRequest { BusinessId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.BusinessId);
        }

        [Fact]
        public void Should_Have_Error_When_HiringDate_Is_Provided_And_Future()
        {
            var model = new UpdateEmployeeRequest { HiringDate = DateTime.Today.AddDays(1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.HiringDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_HiringDate_Is_Provided_And_Valid()
        {
            var model = new UpdateEmployeeRequest { HiringDate = DateTime.Today };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.HiringDate);
        }

        [Fact]
        public void Should_Have_Error_When_TerminationDate_Is_Before_HiringDate()
        {
            var model = new UpdateEmployeeRequest
            {
                HiringDate = DateTime.Today,
                TerminationDate = DateTime.Today.AddDays(-1)
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TerminationDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TerminationDate_Is_On_Or_After_HiringDate()
        {
            var modelSame = new UpdateEmployeeRequest
            {
                HiringDate = DateTime.Today,
                TerminationDate = DateTime.Today
            };
            var modelAfter = new UpdateEmployeeRequest
            {
                HiringDate = DateTime.Today,
                TerminationDate = DateTime.Today.AddDays(1)
            };
            _validator.TestValidate(modelSame).ShouldNotHaveValidationErrorFor(x => x.TerminationDate);
            _validator.TestValidate(modelAfter).ShouldNotHaveValidationErrorFor(x => x.TerminationDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TerminationDate_Is_Provided_But_HiringDate_Is_Null()
        {
            // If HiringDate is not sent (null), we cannot compare; rule is not applied.
            var model = new UpdateEmployeeRequest { TerminationDate = DateTime.Today };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.TerminationDate);
        }

        [Fact]
        public void Should_Have_Error_When_PositionId_Is_Provided_But_Zero()
        {
            var model = new UpdateEmployeeRequest { PositionId = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PositionId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_PositionId_Is_Provided_And_Valid()
        {
            var model = new UpdateEmployeeRequest { PositionId = 3 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.PositionId);
        }

        [Fact]
        public void Should_Have_Error_When_DocumentType_Is_Provided_But_Invalid()
        {
            // An invalid enum value should trigger validation error
            var model = new UpdateEmployeeRequest { DocumentType = (DocumentType)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DocumentType);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DocumentType_Is_Not_Provided()
        {
            // Not providing DocumentType should not trigger validation
            var model = new UpdateEmployeeRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DocumentType);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DocumentNumber_Is_Null_Or_Empty()
        {
            // Null and empty strings are considered "not provided" and should not trigger validation
            var modelNull = new UpdateEmployeeRequest { DocumentNumber = null };
            var modelEmpty = new UpdateEmployeeRequest { DocumentNumber = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_DocumentNumber_Is_Provided_And_Valid()
        {
            // Valid length between 5 and 20
            var model = new UpdateEmployeeRequest { DocumentNumber = "12345678" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DocumentNumber);
        }

        [Fact]
        public void Should_Not_Have_Error_When_SecondSurname_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { SecondSurname = null };
            var modelEmpty = new UpdateEmployeeRequest { SecondSurname = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_SecondSurname_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { SecondSurname = new string('D', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.SecondSurname);
        }

        [Fact]
        public void Should_Not_Have_Error_When_State_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { State = null };
            var modelEmpty = new UpdateEmployeeRequest { State = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_State_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { State = new string('E', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.State);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Municipality_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { Municipality = null };
            var modelEmpty = new UpdateEmployeeRequest { Municipality = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Municipality_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { Municipality = new string('F', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Municipality);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Neighborhood_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { Neighborhood = null };
            var modelEmpty = new UpdateEmployeeRequest { Neighborhood = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Neighborhood_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { Neighborhood = new string('G', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Neighborhood);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AddressStreet_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { AddressStreet = null };
            var modelEmpty = new UpdateEmployeeRequest { AddressStreet = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_Is_Provided_But_Too_Short()
        {
            // Length 4 is less than minimum allowed (5)
            var model = new UpdateEmployeeRequest { AddressStreet = "ABCD" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { AddressStreet = new string('H', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AddressStreet_Is_Provided_And_Valid()
        {
            var model = new UpdateEmployeeRequest { AddressStreet = "Calle 123" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AlternativeAddressStreet_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { AlternativeAddressStreet = null };
            var modelEmpty = new UpdateEmployeeRequest { AlternativeAddressStreet = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_AlternativeAddressStreet_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { AlternativeAddressStreet = new string('I', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AlternativeAddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_PostalCode_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { PostalCode = null };
            var modelEmpty = new UpdateEmployeeRequest { PostalCode = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_PostalCode_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { PostalCode = new string('J', 21) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PostalCode);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Diseases_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { Diseases = null };
            var modelEmpty = new UpdateEmployeeRequest { Diseases = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Diseases_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { Diseases = new string('K', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Diseases);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Medications_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { Medications = null };
            var modelEmpty = new UpdateEmployeeRequest { Medications = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Medications_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { Medications = new string('L', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Medications);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Allergies_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { Allergies = null };
            var modelEmpty = new UpdateEmployeeRequest { Allergies = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Allergies_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { Allergies = new string('M', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Allergies);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Notes_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEmployeeRequest { Notes = null };
            var modelEmpty = new UpdateEmployeeRequest { Notes = "" };

            _validator.TestValidate(modelNull).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(modelEmpty).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Notes_Exceeds_MaxLength()
        {
            var model = new UpdateEmployeeRequest { Notes = new string('N', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Notes);
        }

        [Fact]
        public void Should_Have_Error_When_BranchId_Is_Provided_But_Zero()
        {
            var model = new UpdateEmployeeRequest { BranchId = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BranchId);
        }

        [Fact]
        public void Should_Have_Error_When_BranchId_Is_Provided_But_Negative()
        {
            var model = new UpdateEmployeeRequest { BranchId = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BranchId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_BranchId_Is_Provided_And_Valid()
        {
            var model = new UpdateEmployeeRequest { BranchId = 3 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.BranchId);
        }

        [Fact]
        public void Should_Have_Error_When_HealthPromotionEntityId_Is_Provided_But_Zero()
        {
            var model = new UpdateEmployeeRequest { HealthPromotionEntityId = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.HealthPromotionEntityId);
        }

        [Fact]
        public void Should_Have_Error_When_HealthPromotionEntityId_Is_Provided_But_Negative()
        {
            var model = new UpdateEmployeeRequest { HealthPromotionEntityId = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.HealthPromotionEntityId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_HealthPromotionEntityId_Is_Provided_And_Valid()
        {
            var model = new UpdateEmployeeRequest { HealthPromotionEntityId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.HealthPromotionEntityId);
        }

        [Fact]
        public void Should_Have_Error_When_OccupationalRiskAdministratorId_Is_Provided_But_Zero()
        {
            var model = new UpdateEmployeeRequest { OccupationalRiskAdministratorId = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
        }

        [Fact]
        public void Should_Have_Error_When_OccupationalRiskAdministratorId_Is_Provided_But_Negative()
        {
            var model = new UpdateEmployeeRequest { OccupationalRiskAdministratorId = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_OccupationalRiskAdministratorId_Is_Provided_And_Valid()
        {
            var model = new UpdateEmployeeRequest { OccupationalRiskAdministratorId = 2 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
        }

        [Fact]
        public void Should_Have_Error_When_DepartmentId_Is_Provided_But_Zero()
        {
            var model = new UpdateEmployeeRequest { DepartmentId = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
        }

        [Fact]
        public void Should_Have_Error_When_DepartmentId_Is_Provided_But_Negative()
        {
            var model = new UpdateEmployeeRequest { DepartmentId = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DepartmentId_Is_Provided_And_Valid()
        {
            var model = new UpdateEmployeeRequest { DepartmentId = 4 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DepartmentId);
        }
    }
}
