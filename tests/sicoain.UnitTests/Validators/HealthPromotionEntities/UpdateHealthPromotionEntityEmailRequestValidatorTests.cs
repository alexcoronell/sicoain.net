using FluentValidation.TestHelper;
using sicoain.api.Validators.HealthPromotionEntities;
using sicoain.shared.DTOs.HealthPromotionEntities;
using Xunit;

namespace sicoain.UnitTests.Validators.HealthPromotionEntities
{
    public class UpdateHealthPromotionEntityEmailRequestValidatorTests
    {
        private readonly UpdateHealthPromotionEntityEmailRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateHealthPromotionEntityEmailRequest { Email = null };
            var modelEmpty = new UpdateHealthPromotionEntityEmailRequest { Email = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Email);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Provided_But_Invalid()
        {
            var model = new UpdateHealthPromotionEntityEmailRequest { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Provided_And_Valid()
        {
            var model = new UpdateHealthPromotionEntityEmailRequest { Email = "good@eps.com" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
