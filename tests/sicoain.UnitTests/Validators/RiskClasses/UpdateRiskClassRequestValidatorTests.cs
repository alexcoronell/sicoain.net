using FluentValidation.TestHelper;
using sicoain.api.Validators.RiskClasses;
using sicoain.shared.DTOs.RiskClasses;
using Xunit;

namespace sicoain.UnitTests.Validators.RiskClasses
{
    /// <summary>
    /// Unit tests for the UpdateRiskClassRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateRiskClassRequestValidatorTests
    {
        private readonly UpdateRiskClassRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateRiskClassRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateRiskClassRequest { Name = null };
            var modelEmpty = new UpdateRiskClassRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdateRiskClassRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdateRiskClassRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdateRiskClassRequest { Name = "Updated Risk Class" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Code_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateRiskClassRequest { Code = null };
            var modelEmpty = new UpdateRiskClassRequest { Code = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Code);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_Have_Error_When_Code_Is_Provided_And_Too_Long()
        {
            var model = new UpdateRiskClassRequest { Code = "ABCDEF" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Code_Is_Provided_And_Valid()
        {
            var model = new UpdateRiskClassRequest { Code = "II" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ContributionRate_Is_Not_Provided()
        {
            var model = new UpdateRiskClassRequest { ContributionRate = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ContributionRate);
        }

        [Fact]
        public void Should_Have_Error_When_ContributionRate_Is_Provided_And_Below_Range()
        {
            var model = new UpdateRiskClassRequest { ContributionRate = 0.0000m };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContributionRate);
        }

        [Fact]
        public void Should_Have_Error_When_ContributionRate_Is_Provided_And_Above_Range()
        {
            var model = new UpdateRiskClassRequest { ContributionRate = 10.0000m };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContributionRate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ContributionRate_Is_Provided_And_Within_Range()
        {
            var model = new UpdateRiskClassRequest { ContributionRate = 4.3500m };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ContributionRate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_IsActive_Is_Not_Provided()
        {
            var model = new UpdateRiskClassRequest { IsActive = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.IsActive);
        }

        [Fact]
        public void Should_Not_Have_Error_When_IsActive_Is_Provided_And_Valid()
        {
            var modelTrue = new UpdateRiskClassRequest { IsActive = true };
            var modelFalse = new UpdateRiskClassRequest { IsActive = false };
            _validator.TestValidate(modelTrue).ShouldNotHaveValidationErrorFor(x => x.IsActive);
            _validator.TestValidate(modelFalse).ShouldNotHaveValidationErrorFor(x => x.IsActive);
        }
    }
}
