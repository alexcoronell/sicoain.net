using FluentValidation.TestHelper;
using sicoain.api.Validators.Users;
using sicoain.shared.DTOs.Users;
using Xunit;

namespace sicoain.UnitTests.Validators.Users
{
    public class UpdateUserRequestValidatorTests
    {
        private readonly UpdateUserRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateUserRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateUserRequest { Email = null };
            var modelEmpty = new UpdateUserRequest { Email = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Email);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Provided_But_Invalid()
        {
            var model = new UpdateUserRequest { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Provided_And_Valid()
        {
            var model = new UpdateUserRequest { Email = "good@example.com" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_FullName_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateUserRequest { FullName = null };
            var modelEmpty = new UpdateUserRequest { FullName = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.FullName);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Provided_And_Too_Short()
        {
            var model = new UpdateUserRequest { FullName = "A" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Provided_And_Too_Long()
        {
            var model = new UpdateUserRequest { FullName = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_FullName_Is_Provided_And_Valid()
        {
            var model = new UpdateUserRequest { FullName = "Updated Name" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_IsActive_Is_Provided_As_True_Or_False()
        {
            var modelTrue = new UpdateUserRequest { IsActive = true };
            var modelFalse = new UpdateUserRequest { IsActive = false };
            _validator.TestValidate(modelTrue).ShouldNotHaveValidationErrorFor(x => x.IsActive);
            _validator.TestValidate(modelFalse).ShouldNotHaveValidationErrorFor(x => x.IsActive);
        }
    }
}
