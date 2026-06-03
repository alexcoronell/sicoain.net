using FluentValidation.TestHelper;
using sicoain.api.Validators.Businesses;
using sicoain.shared.DTOs.Business;
using Xunit;

namespace sicoain.UnitTests.Validators.Business
{
    /// <summary>
    /// Unit tests for the CreateBusinessPhoneRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateBusinessPhoneRequestValidatorTests
    {
        private readonly CreateBusinessPhoneRequestValidator _validator = new();

        private CreateBusinessPhoneRequest CreateValidRequest() => new()
        {
            Phone = "3101234567",
            BusinessId = 1
        };

        [Fact]
        public void Should_Have_Error_When_Phone_Is_Empty()
        {
            var model = CreateValidRequest() with { Phone = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456789012345")] // too long
        [InlineData("abc")]
        public void Should_Have_Error_When_Phone_Is_Invalid(string invalidPhone)
        {
            var model = CreateValidRequest() with { Phone = invalidPhone };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("3101234567")]
        [InlineData("+573101234567")]
        [InlineData("573101234567")]
        [InlineData("6012345678")]
        public void Should_Not_Have_Error_When_Phone_Is_Valid(string validPhone)
        {
            var model = CreateValidRequest() with { Phone = validPhone };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Phone);
        }

        [Fact]
        public void Should_Have_Error_When_BusinessId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { BusinessId = 0 };
            var modelNegative = CreateValidRequest() with { BusinessId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.BusinessId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.BusinessId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_BusinessId_Is_Valid()
        {
            var model = CreateValidRequest() with { BusinessId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.BusinessId);
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
