using FluentValidation.TestHelper;
using sicoain.api.Validators;
using sicoain.shared.DTOs.Employees;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.Employees
{
    public class CreateEmployeeRequestValidatorTests
    {
        private readonly CreateEmployeeRequestValidator _validator = new();

        private CreateEmployeeRequest CreateValidRequest() => new()
        {
            DocumentType = DocumentType.CedulaDeCiudadania,
            DocumentNumber = "12345678",
            FirstName = "Juan",
            Surname = "Perez",
            State = "Antioquia",
            Municipality = "Medellin",
            Neighborhood = "Centro",
            AddressStreet = "Calle 123",
            HiringDate = DateTime.Today,
            BusinessId = 1,
            BranchId = 1,
            HealthPromotionEntityId = 1,
            OccupationalRiskAdministratorId = 1,
            DepartmentId = 1,
            PositionId = 1
        };

        [Fact]
        public void Should_Have_Error_When_DocumentNumber_IsEmpty()
        {
            var model = CreateValidRequest() with { DocumentNumber = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")] // too short
        public void Should_Have_Error_When_FirstName_Invalid(string firstName)
        {
            var model = CreateValidRequest() with { FirstName = firstName };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Should_Have_Error_When_HiringDate_IsFuture()
        {
            var model = CreateValidRequest() with { HiringDate = DateTime.Today.AddDays(1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.HiringDate);
        }

        [Fact]
        public void Should_Have_Error_When_TerminationDate_Before_HiringDate()
        {
            var model = CreateValidRequest() with
            {
                HiringDate = DateTime.Today,
                TerminationDate = DateTime.Today.AddDays(-1)
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TerminationDate);
        }

        [Fact]
        public void Should_Have_Error_When_DocumentType_IsInvalid()
        {
            var model = CreateValidRequest() with { DocumentType = (DocumentType)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DocumentType);
        }

        [Theory]
        [InlineData("1234")]                    // too short
        [InlineData("123456789012345678901")]   // too long (21 chars)
        public void Should_Have_Error_When_DocumentNumber_InvalidLength(string documentNumber)
        {
            var model = CreateValidRequest() with { DocumentNumber = documentNumber };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
        }

        [Fact]
        public void Should_Have_Error_When_FirstName_TooLong()
        {
            var model = CreateValidRequest() with { FirstName = new string('A', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Should_Have_Error_When_SecondName_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { SecondName = new string('A', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.SecondName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_SecondName_IsNull()
        {
            var model = CreateValidRequest() with { SecondName = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.SecondName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")] // too short
        public void Should_Have_Error_When_Surname_Invalid(string surname)
        {
            var model = CreateValidRequest() with { Surname = surname };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Surname);
        }

        [Fact]
        public void Should_Have_Error_When_Surname_TooLong()
        {
            var model = CreateValidRequest() with { Surname = new string('A', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Surname);
        }

        [Fact]
        public void Should_Have_Error_When_SecondSurname_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { SecondSurname = new string('A', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.SecondSurname);
        }

        [Fact]
        public void Should_Not_Have_Error_When_SecondSurname_IsNull()
        {
            var model = CreateValidRequest() with { SecondSurname = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.SecondSurname);
        }

        [Fact]
        public void Should_Have_Error_When_State_IsEmpty()
        {
            var model = CreateValidRequest() with { State = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.State);
        }

        [Fact]
        public void Should_Have_Error_When_State_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { State = new string('A', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.State);
        }

        [Fact]
        public void Should_Have_Error_When_Municipality_IsEmpty()
        {
            var model = CreateValidRequest() with { Municipality = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Municipality);
        }

        [Fact]
        public void Should_Have_Error_When_Municipality_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { Municipality = new string('A', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Municipality);
        }

        [Fact]
        public void Should_Have_Error_When_Neighborhood_IsEmpty()
        {
            var model = CreateValidRequest() with { Neighborhood = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Neighborhood);
        }

        [Fact]
        public void Should_Have_Error_When_Neighborhood_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { Neighborhood = new string('A', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Neighborhood);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1234")] // too short
        public void Should_Have_Error_When_AddressStreet_Invalid(string addressStreet)
        {
            var model = CreateValidRequest() with { AddressStreet = addressStreet };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_TooLong()
        {
            var model = CreateValidRequest() with { AddressStreet = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Have_Error_When_AlternativeAddressStreet_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { AlternativeAddressStreet = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AlternativeAddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AlternativeAddressStreet_IsNull()
        {
            var model = CreateValidRequest() with { AlternativeAddressStreet = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AlternativeAddressStreet);
        }

        [Fact]
        public void Should_Have_Error_When_PostalCode_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { PostalCode = new string('A', 21) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PostalCode);
        }

        [Fact]
        public void Should_Not_Have_Error_When_PostalCode_IsNull()
        {
            var model = CreateValidRequest() with { PostalCode = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.PostalCode);
        }

        [Fact]
        public void Should_Have_Error_When_HiringDate_IsEmpty()
        {
            var model = CreateValidRequest() with { HiringDate = default };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.HiringDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TerminationDate_IsNull()
        {
            var model = CreateValidRequest() with { TerminationDate = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.TerminationDate);
        }

        [Fact]
        public void Should_Have_Error_When_Diseases_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { Diseases = new string('A', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Diseases);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Diseases_IsNull()
        {
            var model = CreateValidRequest() with { Diseases = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Diseases);
        }

        [Fact]
        public void Should_Have_Error_When_Medications_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { Medications = new string('A', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Medications);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Medications_IsNull()
        {
            var model = CreateValidRequest() with { Medications = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Medications);
        }

        [Fact]
        public void Should_Have_Error_When_Allergies_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { Allergies = new string('A', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Allergies);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Allergies_IsNull()
        {
            var model = CreateValidRequest() with { Allergies = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Allergies);
        }

        [Fact]
        public void Should_Have_Error_When_Notes_ExceedsMaxLength()
        {
            var model = CreateValidRequest() with { Notes = new string('A', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Notes);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Notes_IsNull()
        {
            var model = CreateValidRequest() with { Notes = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Notes);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_BusinessId_IsZeroOrNegative(int businessId)
        {
            var model = CreateValidRequest() with { BusinessId = businessId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_BranchId_IsZeroOrNegative(int branchId)
        {
            var model = CreateValidRequest() with { BranchId = branchId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BranchId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_HealthPromotionEntityId_IsZeroOrNegative(int healthPromotionEntityId)
        {
            var model = CreateValidRequest() with { HealthPromotionEntityId = healthPromotionEntityId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.HealthPromotionEntityId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_OccupationalRiskAdministratorId_IsZeroOrNegative(int occupationalRiskAdministratorId)
        {
            var model = CreateValidRequest() with { OccupationalRiskAdministratorId = occupationalRiskAdministratorId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_DepartmentId_IsZeroOrNegative(int departmentId)
        {
            var model = CreateValidRequest() with { DepartmentId = departmentId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DepartmentId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_PositionId_IsZeroOrNegative(int positionId)
        {
            var model = CreateValidRequest() with { PositionId = positionId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PositionId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid()
        {
            var model = CreateValidRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
