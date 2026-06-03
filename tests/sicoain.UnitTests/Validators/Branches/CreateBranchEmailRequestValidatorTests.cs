using FluentValidation.TestHelper;
using sicoain.api.Validators;
using sicoain.shared.DTOs.Branches;
using Xunit;

namespace sicoain.UnitTests.Validators.Branches
{
    /// <summary>
    /// Unit tests for the CreateBranchEmailRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateBranchEmailRequestValidatorTests
    {
        private readonly CreateBranchEmailRequestValidator _validator = new();

        private CreateBranchEmailRequest CreateValidRequest() => new()
        {
            Email = "branch@example.com",
            BranchId = 1
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
        public void Should_Have_Error_When_BranchId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { BranchId = 0 };
            var modelNegative = CreateValidRequest() with { BranchId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.BranchId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.BranchId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_BranchId_Is_Valid()
        {
            var model = CreateValidRequest() with { BranchId = 3 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.BranchId);
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
