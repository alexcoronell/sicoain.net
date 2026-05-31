using FluentValidation.TestHelper;
using sicoain.api.Validators.Businesses;
using sicoain.shared.DTOs.Business;
using Xunit;

namespace sicoain.UnitTests.Validators.Business
{
    public class CreateBusinessRequestValidatorTests
    {
        private readonly CreateBusinessRequestValidator _validator = new();

        private CreateBusinessRequest CreateValidRequest() => new()
        {
            Name = "Test Business",
            AddressStreet = "123 Main St"
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
            var model = CreateValidRequest() with { Name = "Valid Name" };
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
        public void Should_Not_Have_Error_When_Valid()
        {
            var model = CreateValidRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
