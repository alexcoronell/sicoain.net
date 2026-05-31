using FluentValidation.TestHelper;
using sicoain.api.Validators.Businesses;
using sicoain.shared.DTOs.Business;
using Xunit;

namespace sicoain.UnitTests.Validators.Business
{
    public class UpdateBusinessRequestValidatorTests
    {
        private readonly UpdateBusinessRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateBusinessRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateBusinessRequest { Name = null };
            var modelEmpty = new UpdateBusinessRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdateBusinessRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdateBusinessRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdateBusinessRequest { Name = "Updated Name" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_Is_Provided_And_Too_Long()
        {
            var model = new UpdateBusinessRequest { AddressStreet = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AddressStreet_Is_Provided_And_Valid()
        {
            var model = new UpdateBusinessRequest { AddressStreet = "123 New Address" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
        }
    }
}
