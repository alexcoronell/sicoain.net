using FluentValidation.TestHelper;
using sicoain.api.Validators.HealthPromotionEntities;
using sicoain.shared.DTOs.HealthPromotionEntities;
using Xunit;

namespace sicoain.UnitTests.Validators.HealthPromotionEntities
{
    public class UpdateHealthPromotionEntityRequestValidatorTests
    {
        private readonly UpdateHealthPromotionEntityRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateHealthPromotionEntityRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateHealthPromotionEntityRequest { Name = null };
            var modelEmpty = new UpdateHealthPromotionEntityRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdateHealthPromotionEntityRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdateHealthPromotionEntityRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdateHealthPromotionEntityRequest { Name = "Updated EPS" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_Is_Provided_And_Too_Long()
        {
            var model = new UpdateHealthPromotionEntityRequest { AddressStreet = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AddressStreet_Is_Provided_And_Valid()
        {
            var model = new UpdateHealthPromotionEntityRequest { AddressStreet = "New address" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Have_Error_When_Notes_Is_Provided_And_Too_Long()
        {
            var model = new UpdateHealthPromotionEntityRequest { Notes = new string('N', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Notes);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Notes_Is_Provided_And_Valid()
        {
            var model = new UpdateHealthPromotionEntityRequest { Notes = "Short note" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Notes);
        }
    }
}
