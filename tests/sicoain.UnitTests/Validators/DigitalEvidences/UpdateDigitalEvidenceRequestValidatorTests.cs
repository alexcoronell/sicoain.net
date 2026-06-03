using FluentValidation.TestHelper;
using sicoain.api.Validators.DigitalEvidences;
using sicoain.shared.DTOs.DigitalEvidences;
using Xunit;

namespace sicoain.UnitTests.Validators.DigitalEvidences
{
    /// <summary>
    /// Unit tests for the UpdateDigitalEvidenceRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateDigitalEvidenceRequestValidatorTests
    {
        private readonly UpdateDigitalEvidenceRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateDigitalEvidenceRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_FileName_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateDigitalEvidenceRequest { FileName = null };
            var modelEmpty = new UpdateDigitalEvidenceRequest { FileName = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.FileName);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Should_Have_Error_When_FileName_Is_Provided_And_Too_Long()
        {
            var model = new UpdateDigitalEvidenceRequest { FileName = new string('F', 256) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_FileName_Is_Provided_And_Valid()
        {
            var model = new UpdateDigitalEvidenceRequest { FileName = "new-file.jpg" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FileName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateDigitalEvidenceRequest { Description = null };
            var modelEmpty = new UpdateDigitalEvidenceRequest { Description = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Description);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Long()
        {
            var model = new UpdateDigitalEvidenceRequest { Description = new string('D', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Provided_And_Valid()
        {
            var model = new UpdateDigitalEvidenceRequest { Description = "Updated description" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TakenAt_Is_Not_Provided()
        {
            var model = new UpdateDigitalEvidenceRequest { TakenAt = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.TakenAt);
        }

        [Fact]
        public void Should_Have_Error_When_TakenAt_Is_Provided_And_Future()
        {
            var model = new UpdateDigitalEvidenceRequest { TakenAt = DateTime.UtcNow.AddDays(1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TakenAt);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TakenAt_Is_Provided_And_Valid()
        {
            var model = new UpdateDigitalEvidenceRequest { TakenAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc) };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.TakenAt);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TakenByName_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateDigitalEvidenceRequest { TakenByName = null };
            var modelEmpty = new UpdateDigitalEvidenceRequest { TakenByName = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.TakenByName);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.TakenByName);
        }

        [Fact]
        public void Should_Have_Error_When_TakenByName_Is_Provided_And_Too_Long()
        {
            var model = new UpdateDigitalEvidenceRequest { TakenByName = new string('N', 151) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TakenByName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_TakenByName_Is_Provided_And_Valid()
        {
            var model = new UpdateDigitalEvidenceRequest { TakenByName = "Valid name" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.TakenByName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ChainOfCustody_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateDigitalEvidenceRequest { ChainOfCustody = null };
            var modelEmpty = new UpdateDigitalEvidenceRequest { ChainOfCustody = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.ChainOfCustody);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.ChainOfCustody);
        }

        [Fact]
        public void Should_Have_Error_When_ChainOfCustody_Is_Provided_And_Too_Long()
        {
            var model = new UpdateDigitalEvidenceRequest { ChainOfCustody = new string('C', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ChainOfCustody);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ChainOfCustody_Is_Provided_And_Valid()
        {
            var model = new UpdateDigitalEvidenceRequest { ChainOfCustody = "Valid chain of custody entry" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ChainOfCustody);
        }
    }
}
