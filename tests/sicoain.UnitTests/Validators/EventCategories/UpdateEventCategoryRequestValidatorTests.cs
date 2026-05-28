using FluentValidation.TestHelper;
using sicoain.api.Validators.EventCategories;
using sicoain.shared.DTOs.EventCategories;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.EventCategories
{
    public class UpdateEventCategoryRequestValidatorTests
    {
        private readonly UpdateEventCategoryRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateEventCategoryRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateEventCategoryRequest { Name = null };
            var modelEmpty = new UpdateEventCategoryRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdateEventCategoryRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdateEventCategoryRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdateEventCategoryRequest { Name = "Updated Category" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_LevelOfSeverity_Is_Not_Provided()
        {
            var model = new UpdateEventCategoryRequest { LevelOfSeverity = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.LevelOfSeverity);
        }

        [Fact]
        public void Should_Have_Error_When_LevelOfSeverity_Is_Provided_And_Invalid()
        {
            var model = new UpdateEventCategoryRequest { LevelOfSeverity = (AccidentSeverity)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.LevelOfSeverity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_LevelOfSeverity_Is_Provided_And_Valid()
        {
            var model = new UpdateEventCategoryRequest { LevelOfSeverity = AccidentSeverity.Critico };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.LevelOfSeverity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Only_RequiresHospitalization_Is_Provided()
        {
            // RequiresHospitalization is a non-nullable bool, so it's always sent.
            // No validation rule applies to it.
            var model = new UpdateEventCategoryRequest { RequiresHospitalization = true };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
