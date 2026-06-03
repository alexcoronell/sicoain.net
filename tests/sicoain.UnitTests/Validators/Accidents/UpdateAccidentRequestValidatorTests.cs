using FluentValidation.TestHelper;
using sicoain.api.Validators.Accidents;
using sicoain.shared.DTOs.Accident;
using Xunit;

namespace sicoain.UnitTests.Validators.Accidents
{
    /// <summary>
    /// Unit tests for the UpdateAccidentRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateAccidentRequestValidatorTests
    {
        private readonly UpdateAccidentRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Null()
        {
            var model = new UpdateAccidentRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_EventDate_Is_Not_Provided()
        {
            var model = new UpdateAccidentRequest { EventDate = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EventDate);
        }

        [Fact]
        public void Should_Have_Error_When_EventDate_Is_Provided_And_Future()
        {
            var model = new UpdateAccidentRequest { EventDate = DateTime.Today.AddDays(1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EventDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EventDate_Is_Provided_And_Valid()
        {
            var model = new UpdateAccidentRequest { EventDate = DateTime.Today };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EventDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateAccidentRequest { Description = null };
            var modelEmpty = new UpdateAccidentRequest { Description = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Description);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Short()
        {
            var model = new UpdateAccidentRequest { Description = new string('A', 9) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Long()
        {
            var model = new UpdateAccidentRequest { Description = new string('A', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Provided_And_Valid()
        {
            var model = new UpdateAccidentRequest { Description = new string('A', 50) };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EmployeeId_Is_Not_Provided()
        {
            var model = new UpdateAccidentRequest { EmployeeId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_EmployeeId_Is_Provided_And_Invalid(int invalidId)
        {
            var model = new UpdateAccidentRequest { EmployeeId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EmployeeId_Is_Provided_And_Valid()
        {
            var model = new UpdateAccidentRequest { EmployeeId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EmployeeId);
        }

        // Similar tests for AccidentTypeId and EventCategoryId (follow same pattern)
        [Fact]
        public void Should_Not_Have_Error_When_AccidentTypeId_Is_Not_Provided()
        {
            var model = new UpdateAccidentRequest { AccidentTypeId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentTypeId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_AccidentTypeId_Is_Provided_And_Invalid(int invalidId)
        {
            var model = new UpdateAccidentRequest { AccidentTypeId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AccidentTypeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentTypeId_Is_Provided_And_Valid()
        {
            var model = new UpdateAccidentRequest { AccidentTypeId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentTypeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EventCategoryId_Is_Not_Provided()
        {
            var model = new UpdateAccidentRequest { EventCategoryId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EventCategoryId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_EventCategoryId_Is_Provided_And_Invalid(int invalidId)
        {
            var model = new UpdateAccidentRequest { EventCategoryId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EventCategoryId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EventCategoryId_Is_Provided_And_Valid()
        {
            var model = new UpdateAccidentRequest { EventCategoryId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EventCategoryId);
        }
    }
}
