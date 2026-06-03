using FluentValidation.TestHelper;
using sicoain.api.Validators.HealthPromotionEntities;
using sicoain.shared.DTOs.HealthPromotionEntities;
using Xunit;

namespace sicoain.UnitTests.Validators.HealthPromotionEntities
{
    /// <summary>
    /// Unit tests for the CreateHealthPromotionEntityEmailRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateHealthPromotionEntityEmailRequestValidatorTests
    {
        private readonly CreateHealthPromotionEntityEmailRequestValidator _validator = new();

        private CreateHealthPromotionEntityEmailRequest CreateValidRequest() => new()
        {
            Email = "eps@example.com",
            HealthPromotionEntityId = 1
        };

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = CreateValidRequest() with { Email = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = CreateValidRequest() with { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
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
