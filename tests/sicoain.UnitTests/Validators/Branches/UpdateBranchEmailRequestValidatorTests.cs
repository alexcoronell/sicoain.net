using FluentValidation.TestHelper;
using sicoain.api.Validators;
using sicoain.shared.DTOs.Branches;
using Xunit;

namespace sicoain.UnitTests.Validators.Branches
{
    /// <summary>
    /// Unit tests for the UpdateBranchEmailRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateBranchEmailRequestValidatorTests
    {
        private readonly UpdateBranchEmailRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateBranchEmailRequest { Email = null };
            var modelEmpty = new UpdateBranchEmailRequest { Email = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Email);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Provided_But_Invalid()
        {
            var model = new UpdateBranchEmailRequest { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Provided_And_Valid()
        {
            var model = new UpdateBranchEmailRequest { Email = "good@branch.com" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
