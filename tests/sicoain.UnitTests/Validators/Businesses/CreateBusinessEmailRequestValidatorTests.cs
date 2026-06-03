using FluentValidation.TestHelper;
using sicoain.api.Validators.Businesses;
using sicoain.shared.DTOs.Business;
using Xunit;

namespace sicoain.UnitTests.Validators.Business
{
    /// <summary>
    /// Unit tests for the CreateBusinessEmailRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateBusinessEmailRequestValidatorTests
    {
        private readonly CreateBusinessEmailRequestValidator _validator = new();

        private CreateBusinessEmailRequest CreateValidRequest() => new()
        {
            Email = "test@example.com",
            BusinessId = 1
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
            var model = CreateValidRequest() with { Email = "not-an-email" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
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
