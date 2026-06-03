using FluentValidation.TestHelper;
using sicoain.api.Validators.AccidentTypes;
using sicoain.shared.DTOs.AccidentTypes;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.AccidentTypes
{
    /// <summary>
    /// Unit tests for the CreateAccidentTypeRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateAccidentTypeRequestValidatorTests
    {
        private readonly CreateAccidentTypeRequestValidator _validator = new();

        private CreateAccidentTypeRequest CreateValidRequest() => new()
        {
            Name = "Fracture",
            Description = "Bone fracture",
            Severity = AccidentSeverity.Severe
        };

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = CreateValidRequest() with { Name = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            var model = CreateValidRequest() with { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Long()
        {
            var model = CreateValidRequest() with { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Valid()
        {
            var model = CreateValidRequest() with { Name = "Laceration" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { Description = new string('D', 251) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null_Or_Within_Limit()
        {
            var modelNull = CreateValidRequest() with { Description = null };
            var modelValid = CreateValidRequest() with { Description = new string('D', 250) };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Description);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Severity_Is_Invalid()
        {
            var model = CreateValidRequest() with { Severity = (AccidentSeverity)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Severity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Severity_Is_Valid()
        {
            var model = CreateValidRequest() with { Severity = AccidentSeverity.Mild };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Severity);
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
