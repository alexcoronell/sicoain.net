using FluentValidation.TestHelper;
using sicoain.api.Validators.Attachments;
using sicoain.shared.DTOs.Attachments;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.Attachments
{
    public class CreateAttachmentRequestValidatorTests
    {
        private readonly CreateAttachmentRequestValidator _validator = new();

        private CreateAttachmentRequest CreateValidRequest() => new()
        {
            FileName = "document.pdf",
            MimeType = "application/pdf",
            Description = "A test document",
            EntityType = AttachmentEntityType.Accident,
            EntityId = 1,
            Base64Content = Convert.ToBase64String(new byte[] { 0x01, 0x02, 0x03 })
        };

        [Fact]
        public void Should_Have_Error_When_FileName_Is_Empty()
        {
            var model = CreateValidRequest() with { FileName = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Should_Have_Error_When_FileName_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { FileName = new string('F', 256) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_FileName_Is_Valid()
        {
            var model = CreateValidRequest() with { FileName = "image.jpg" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Should_Have_Error_When_MimeType_Is_Empty()
        {
            var model = CreateValidRequest() with { MimeType = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MimeType);
        }

        [Fact]
        public void Should_Have_Error_When_MimeType_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { MimeType = new string('M', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MimeType);
        }

        [Fact]
        public void Should_Not_Have_Error_When_MimeType_Is_Valid()
        {
            var model = CreateValidRequest() with { MimeType = "image/png" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.MimeType);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { Description = new string('D', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null_Or_Within_Limit()
        {
            var modelNull = CreateValidRequest() with { Description = null };
            var modelValid = CreateValidRequest() with { Description = new string('D', 500) };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Description);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_EntityType_Is_Invalid()
        {
            var model = CreateValidRequest() with { EntityType = (AttachmentEntityType)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EntityType);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EntityType_Is_Valid()
        {
            var model = CreateValidRequest() with { EntityType = AttachmentEntityType.Employee };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EntityType);
        }

        [Fact]
        public void Should_Have_Error_When_EntityId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { EntityId = 0 };
            var modelNegative = CreateValidRequest() with { EntityId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.EntityId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.EntityId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EntityId_Is_Positive()
        {
            var model = CreateValidRequest() with { EntityId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EntityId);
        }

        [Fact]
        public void Should_Have_Error_When_Base64Content_Is_Empty()
        {
            var model = CreateValidRequest() with { Base64Content = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Base64Content);
        }

        [Fact]
        public void Should_Have_Error_When_Base64Content_Is_Invalid()
        {
            var model = CreateValidRequest() with { Base64Content = "invalid-base64!!!" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Base64Content);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Base64Content_Is_Valid()
        {
            var model = CreateValidRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Base64Content);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Valid()
        {
            var model = CreateValidRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
