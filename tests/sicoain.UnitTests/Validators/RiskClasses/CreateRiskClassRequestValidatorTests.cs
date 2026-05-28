using FluentValidation.TestHelper;
using sicoain.api.Validators.RiskClasses;
using sicoain.shared.DTOs.RiskClasses;
using Xunit;

namespace sicoain.UnitTests.Validators.RiskClasses
{
    public class CreateRiskClassRequestValidatorTests
    {
        private readonly CreateRiskClassRequestValidator _validator = new();

        private CreateRiskClassRequest CreateValidRequest() => new()
        {
            Name = "High Risk",
            Code = "V",
            ContributionRate = 6.9600m
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
            var model = CreateValidRequest() with { Name = "Moderate Risk" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Code_Is_Empty()
        {
            var model = CreateValidRequest() with { Code = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_Have_Error_When_Code_Is_Too_Long()
        {
            var model = CreateValidRequest() with { Code = "ABCDEF" }; // length 6 > 5
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Code_Is_Valid()
        {
            var model = CreateValidRequest() with { Code = "III" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Code);
        }

        [Fact]
        public void Should_Have_Error_When_ContributionRate_Is_Less_Than_Minimum()
        {
            var model = CreateValidRequest() with { ContributionRate = 0.0000m };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContributionRate);
        }

        [Fact]
        public void Should_Have_Error_When_ContributionRate_Exceeds_Maximum()
        {
            var model = CreateValidRequest() with { ContributionRate = 10.0000m };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ContributionRate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ContributionRate_Is_Within_Range()
        {
            var model = CreateValidRequest() with { ContributionRate = 2.4360m };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ContributionRate);
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
