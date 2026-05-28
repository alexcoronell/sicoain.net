using FluentValidation.TestHelper;
using sicoain.api.Validators.Users;
using sicoain.shared.DTOs.Users;
using Xunit;

namespace sicoain.UnitTests.Validators.Users
{
    public class ChangePasswordRequestValidatorTests
    {
        private readonly ChangePasswordRequestValidator _validator = new();

        private ChangePasswordRequest CreateValidRequest() => new()
        {
            CurrentPassword = "OldPass123!",
            NewPassword = "NewPass123!",
            ConfirmNewPassword = "NewPass123!"
        };

        [Fact]
        public void Should_Have_Error_When_CurrentPassword_Is_Empty()
        {
            var model = CreateValidRequest() with { CurrentPassword = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
        }

        [Fact]
        public void Should_Have_Error_When_NewPassword_Is_Empty()
        {
            var model = CreateValidRequest() with { NewPassword = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Fact]
        public void Should_Have_Error_When_NewPassword_Is_Too_Short()
        {
            var model = CreateValidRequest() with { NewPassword = "Short" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Fact]
        public void Should_Have_Error_When_ConfirmNewPassword_Is_Empty()
        {
            var model = CreateValidRequest() with { ConfirmNewPassword = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
        }

        [Fact]
        public void Should_Have_Error_When_ConfirmNewPassword_Does_Not_Match()
        {
            var model = CreateValidRequest() with { ConfirmNewPassword = "Different123!" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
        }

        [Fact]
        public void Should_Not_Have_Error_When_CurrentPassword_Is_Valid()
        {
            var model = CreateValidRequest() with { CurrentPassword = "ValidOldP@ss" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.CurrentPassword);
        }

        [Fact]
        public void Should_Not_Have_Error_When_NewPassword_Is_Valid()
        {
            var model = CreateValidRequest() with { NewPassword = "AnotherV@lid1" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
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
