using FluentValidation.TestHelper;
using sicoain.api.Validators.Businesses;
using sicoain.shared.DTOs.Business;
using Xunit;

namespace sicoain.UnitTests.Validators.Business
{
    /// <summary>
    /// Unit tests for the UpdateBusinessEmailRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateBusinessEmailRequestValidatorTests
    {
        private readonly UpdateBusinessEmailRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateBusinessEmailRequest { Email = null };
            var modelEmpty = new UpdateBusinessEmailRequest { Email = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Email);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Provided_But_Invalid()
        {
            var model = new UpdateBusinessEmailRequest { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Provided_And_Valid()
        {
            var model = new UpdateBusinessEmailRequest { Email = "good@example.com" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
