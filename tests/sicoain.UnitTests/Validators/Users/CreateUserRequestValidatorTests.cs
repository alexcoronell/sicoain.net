using FluentValidation.TestHelper;
using sicoain.api.Validators.Users;
using sicoain.shared.DTOs.Users;
using Xunit;

namespace sicoain.UnitTests.Validators.Users
{
    public class CreateUserRequestValidatorTests
    {
        private readonly CreateUserRequestValidator _validator = new();

        private CreateUserRequest CreateValidRequest() => new()
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            FullName = "John Doe"
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
            var model = CreateValidRequest() with { Email = "invalid-email" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var model = CreateValidRequest() with { Password = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            var model = CreateValidRequest() with { Password = "Short" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Empty()
        {
            var model = CreateValidRequest() with { FullName = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Too_Short()
        {
            var model = CreateValidRequest() with { FullName = "A" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Too_Long()
        {
            var model = CreateValidRequest() with { FullName = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            var model = CreateValidRequest() with { Email = "user@domain.com" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Password_Is_Valid()
        {
            var model = CreateValidRequest() with { Password = "MySecureP@ss1" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_FullName_Is_Valid()
        {
            var model = CreateValidRequest() with { FullName = "Jane Smith" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FullName);
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
