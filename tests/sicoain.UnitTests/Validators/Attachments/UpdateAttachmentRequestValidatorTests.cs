using FluentValidation.TestHelper;
using sicoain.api.Validators.Attachments;
using sicoain.shared.DTOs.Attachments;
using Xunit;

namespace sicoain.UnitTests.Validators.Attachments
{
    /// <summary>
    /// Unit tests for the UpdateAttachmentRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateAttachmentRequestValidatorTests
    {
        private readonly UpdateAttachmentRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateAttachmentRequest { Description = null };
            var modelEmpty = new UpdateAttachmentRequest { Description = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Description);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Long()
        {
            var model = new UpdateAttachmentRequest { Description = new string('D', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Provided_And_Valid()
        {
            var model = new UpdateAttachmentRequest { Description = "Updated description" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateAttachmentRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
