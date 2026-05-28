using FluentValidation.TestHelper;
using sicoain.api.Validators.EventCategories;
using sicoain.shared.DTOs.EventCategories;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.EventCategories
{
    public class CreateEventCategoryRequestValidatorTests
    {
        private readonly CreateEventCategoryRequestValidator _validator = new();

        private CreateEventCategoryRequest CreateValidRequest() => new()
        {
            Name = "Fall Accident",
            LevelOfSeverity = AccidentSeverity.Moderate
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
            var model = CreateValidRequest() with { Name = "Valid Category" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_LevelOfSeverity_Is_Invalid()
        {
            var model = CreateValidRequest() with { LevelOfSeverity = (AccidentSeverity)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.LevelOfSeverity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_LevelOfSeverity_Is_Valid()
        {
            var model = CreateValidRequest() with { LevelOfSeverity = AccidentSeverity.Severe };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.LevelOfSeverity);
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
