using FluentValidation.TestHelper;
using sicoain.api.Validators.Positions;
using sicoain.shared.DTOs.Positions;

namespace sicoain.UnitTests.Validators.Positions
{
    /// <summary>
    /// Unit tests for the UpdatePositionRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdatePositionRequestValidatorTests
    {
        private readonly UpdatePositionRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdatePositionRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdatePositionRequest { Name = null };
            var modelEmpty = new UpdatePositionRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdatePositionRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdatePositionRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdatePositionRequest { Name = "Senior Developer" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Long()
        {
            var model = new UpdatePositionRequest { Description = new string('D', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Provided_And_Valid()
        {
            var model = new UpdatePositionRequest { Description = "Updated description" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DepartmentId_Is_Not_Provided()
        {
            var model = new UpdatePositionRequest { DepartmentId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DepartmentId);
        }

        [Fact]
        public void Should_Have_Error_When_DepartmentId_Is_Provided_And_ZeroOrNegative()
        {
            var modelZero = new UpdatePositionRequest { DepartmentId = 0 };
            var modelNegative = new UpdatePositionRequest { DepartmentId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.DepartmentId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.DepartmentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DepartmentId_Is_Provided_And_Positive()
        {
            var model = new UpdatePositionRequest { DepartmentId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DepartmentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_RiskClassId_Is_Not_Provided()
        {
            var model = new UpdatePositionRequest { RiskClassId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.RiskClassId);
        }

        [Fact]
        public void Should_Have_Error_When_RiskClassId_Is_Provided_And_ZeroOrNegative()
        {
            var modelZero = new UpdatePositionRequest { RiskClassId = 0 };
            var modelNegative = new UpdatePositionRequest { RiskClassId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.RiskClassId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.RiskClassId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_RiskClassId_Is_Provided_And_Positive()
        {
            var model = new UpdatePositionRequest { RiskClassId = 3 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.RiskClassId);
        }
    }
}
