using FluentValidation.TestHelper;
using sicoain.api.Validators.DigitalEvidences;
using sicoain.shared.DTOs.DigitalEvidences;
using Xunit;

namespace sicoain.UnitTests.Validators.DigitalEvidences
{
    public class CreateDigitalEvidenceRequestValidatorTests
    {
        private readonly CreateDigitalEvidenceRequestValidator _validator = new();

        private CreateDigitalEvidenceRequest CreateValidRequest() => new()
        {
            FileName = "evidence.jpg",
            MimeType = "image/jpeg",
            Description = "Accident scene photo",
            TakenAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            TakenByName = "Officer Smith",
            ChainOfCustody = "Seized from scene",
            AccidentId = 5,
            Base64Content = Convert.ToBase64String(new byte[] { 0x01, 0x02 })
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
            var model = CreateValidRequest() with { FileName = "video.mp4" };
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
            var model = CreateValidRequest() with { MimeType = "video/mp4" };
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
        public void Should_Have_Error_When_TakenAt_Is_Default()
        {
            // DateTime default is DateTime.MinValue, which is not required to be in the future,
            // but NotEmpty will fail because it's considered empty? Actually NotEmpty on DateTime
            // checks if value equals default(DateTime). So we test that.
            var model = CreateValidRequest() with { TakenAt = default(DateTime) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TakenAt);
        }

        [Fact]
        public void Should_Have_Error_When_TakenAt_Is_In_Future()
        {
            var model = CreateValidRequest() with { TakenAt = DateTime.UtcNow.AddDays(1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TakenAt);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TakenAt_Is_Valid()
        {
            var model = CreateValidRequest() with { TakenAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc) };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.TakenAt);
        }

        [Fact]
        public void Should_Have_Error_When_TakenByName_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { TakenByName = new string('N', 151) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TakenByName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TakenByName_Is_Null_Or_Valid()
        {
            var modelNull = CreateValidRequest() with { TakenByName = null };
            var modelValid = CreateValidRequest() with { TakenByName = "Valid name" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.TakenByName);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.TakenByName);
        }

        [Fact]
        public void Should_Have_Error_When_ChainOfCustody_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { ChainOfCustody = new string('C', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ChainOfCustody);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ChainOfCustody_Is_Empty_Or_Valid()
        {
            var modelEmpty = CreateValidRequest() with { ChainOfCustody = "" };
            var modelValid = CreateValidRequest() with { ChainOfCustody = new string('C', 500) };
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.ChainOfCustody);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.ChainOfCustody);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentId_Is_Null()
        {
            var model = CreateValidRequest() with { AccidentId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Have_Error_When_AccidentId_Is_Provided_And_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { AccidentId = 0 };
            var modelNegative = CreateValidRequest() with { AccidentId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.AccidentId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentId_Is_Provided_And_Positive()
        {
            var model = CreateValidRequest() with { AccidentId = 3 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentId);
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
            var model = CreateValidRequest() with { Base64Content = "!@#$%invalid" };
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
