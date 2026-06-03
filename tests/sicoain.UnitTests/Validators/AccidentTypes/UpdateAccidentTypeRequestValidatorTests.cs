using FluentValidation.TestHelper;
using sicoain.api.Validators.AccidentTypes;
using sicoain.shared.DTOs.AccidentTypes;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.AccidentTypes
{
    /// <summary>
    /// Unit tests for the UpdateAccidentTypeRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateAccidentTypeRequestValidatorTests
    {
        private readonly UpdateAccidentTypeRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateAccidentTypeRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateAccidentTypeRequest { Name = null };
            var modelEmpty = new UpdateAccidentTypeRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdateAccidentTypeRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdateAccidentTypeRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdateAccidentTypeRequest { Name = "Updated Type" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Long()
        {
            var model = new UpdateAccidentTypeRequest { Description = new string('D', 251) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Provided_And_Valid()
        {
            var model = new UpdateAccidentTypeRequest { Description = "Valid description" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Severity_Is_Provided_And_Invalid()
        {
            // Severity is non-nullable, so it's always provided; but we test invalid enum value.
            var model = new UpdateAccidentTypeRequest { Severity = (AccidentSeverity)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Severity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Severity_Is_Provided_And_Valid()
        {
            var model = new UpdateAccidentTypeRequest { Severity = AccidentSeverity.Moderate };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Severity);
        }
    }
}
