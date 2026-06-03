using FluentValidation.TestHelper;
using sicoain.api.Validators.HealthPromotionEntities;
using sicoain.shared.DTOs.HealthPromotionEntities;
using Xunit;

namespace sicoain.UnitTests.Validators.HealthPromotionEntities
{
    /// <summary>
    /// Unit tests for the CreateHealthPromotionEntityRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateHealthPromotionEntityRequestValidatorTests
    {
        private readonly CreateHealthPromotionEntityRequestValidator _validator = new();

        private CreateHealthPromotionEntityRequest CreateValidRequest() => new()
        {
            Name = "Sura EPS",
            AddressStreet = "Calle 123",
            Notes = "Some notes"
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
            var model = CreateValidRequest() with { Name = "Compensar EPS" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { AddressStreet = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AddressStreet_Is_Null_Or_Within_Limit()
        {
            var modelNull = CreateValidRequest() with { AddressStreet = null };
            var modelValid = CreateValidRequest() with { AddressStreet = new string('A', 200) };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Have_Error_When_Notes_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { Notes = new string('N', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Notes);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Notes_Is_Null_Or_Within_Limit()
        {
            var modelNull = CreateValidRequest() with { Notes = null };
            var modelValid = CreateValidRequest() with { Notes = new string('N', 500) };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Notes);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.Notes);
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
