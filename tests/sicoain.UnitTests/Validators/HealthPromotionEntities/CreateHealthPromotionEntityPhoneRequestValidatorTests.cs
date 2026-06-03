using FluentValidation.TestHelper;
using sicoain.api.Validators.HealthPromotionEntities;
using sicoain.shared.DTOs.HealthPromotionEntities;
using Xunit;

namespace sicoain.UnitTests.Validators.HealthPromotionEntities
{
    /// <summary>
    /// Unit tests for the CreateHealthPromotionEntityPhoneRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateHealthPromotionEntityPhoneRequestValidatorTests
    {
        private readonly CreateHealthPromotionEntityPhoneRequestValidator _validator = new();

        private CreateHealthPromotionEntityPhoneRequest CreateValidRequest() => new()
        {
            Phone = "3101234567",
            HealthPromotionEntityId = 1
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
        [InlineData("123456789012345")]
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
        public void Should_Have_Error_When_HealthPromotionEntityId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { HealthPromotionEntityId = 0 };
            var modelNegative = CreateValidRequest() with { HealthPromotionEntityId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.HealthPromotionEntityId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.HealthPromotionEntityId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_HealthPromotionEntityId_Is_Valid()
        {
            var model = CreateValidRequest() with { HealthPromotionEntityId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.HealthPromotionEntityId);
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
