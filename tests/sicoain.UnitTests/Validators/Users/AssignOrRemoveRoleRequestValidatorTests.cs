using FluentValidation.TestHelper;
using sicoain.api.Validators.Users;
using sicoain.shared.DTOs.Users;
using Xunit;

namespace sicoain.UnitTests.Validators.Users
{
    public class AssignOrRemoveRoleRequestValidatorTests
    {
        private readonly AssignOrRemoveRoleRequestValidator _validator = new();

        private AssignOrRemoveRoleRequest CreateValidRequest() => new()
        {
            RoleName = "Admin"
        };

        [Fact]
        public void Should_Have_Error_When_RoleName_Is_Empty()
        {
            var model = CreateValidRequest() with { RoleName = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Fact]
        public void Should_Have_Error_When_RoleName_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { RoleName = new string('R', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_RoleName_Is_Valid()
        {
            var model = CreateValidRequest() with { RoleName = "Supervisor" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.RoleName);
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
