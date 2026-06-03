using FluentValidation.TestHelper;
using sicoain.api.Validators.Positions;
using sicoain.shared.DTOs.Positions;

namespace sicoain.UnitTests.Validators.Positions
{
    /// <summary>
    /// Unit tests for the CreatePositionRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreatePositionRequestValidatorTests
    {
        private readonly CreatePositionRequestValidator _validator = new();

        private CreatePositionRequest CreateValidRequest() => new()
        {
            Name = "Software Engineer",
            Description = "Develops and maintains software",
            DepartmentId = 1,
            RiskClassId = 2
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
            var model = CreateValidRequest() with { Name = "Manager" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { Description = new string('D', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null_Or_Within_Limit()
        {
            var modelNull = CreateValidRequest() with { Description = null };
            var modelValid = CreateValidRequest() with { Description = new string('D', 500) };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Description);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_DepartmentId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { DepartmentId = 0 };
            var modelNegative = CreateValidRequest() with { DepartmentId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.DepartmentId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.DepartmentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DepartmentId_Is_Positive()
        {
            var model = CreateValidRequest() with { DepartmentId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DepartmentId);
        }

        [Fact]
        public void Should_Have_Error_When_RiskClassId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { RiskClassId = 0 };
            var modelNegative = CreateValidRequest() with { RiskClassId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.RiskClassId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.RiskClassId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_RiskClassId_Is_Positive()
        {
            var model = CreateValidRequest() with { RiskClassId = 3 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.RiskClassId);
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
