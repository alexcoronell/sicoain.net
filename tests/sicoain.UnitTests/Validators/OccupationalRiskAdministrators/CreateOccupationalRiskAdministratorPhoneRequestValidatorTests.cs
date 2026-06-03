using FluentValidation.TestHelper;
using sicoain.api.Validators.OccupationalRiskAdministrators;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using Xunit;

namespace sicoain.UnitTests.Validators.OccupationalRiskAdministrators
{
    /// <summary>
    /// Unit tests for the CreateOccupationalRiskAdministratorPhoneRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateOccupationalRiskAdministratorPhoneRequestValidatorTests
    {
        private readonly CreateOccupationalRiskAdministratorPhoneRequestValidator _validator = new();

        private CreateOccupationalRiskAdministratorPhoneRequest CreateValidRequest() => new()
        {
            Phone = "3101234567",
            OccupationalRiskAdministratorId = 1
        };

        [Fact]
        public void Should_Have_Error_When_Phone_Is_Empty()
        {
            var model = CreateValidRequest() with { Phone = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456789012345")]
        [InlineData("abc")]
        public void Should_Have_Error_When_Phone_Is_Invalid(string invalidPhone)
        {
            var model = CreateValidRequest() with { Phone = invalidPhone };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("3101234567")]
        [InlineData("+573101234567")]
        [InlineData("573101234567")]
        [InlineData("6012345678")]
        public void Should_Not_Have_Error_When_Phone_Is_Valid(string validPhone)
        {
            var model = CreateValidRequest() with { Phone = validPhone };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Phone);
        }

        [Fact]
        public void Should_Have_Error_When_OccupationalRiskAdministratorId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { OccupationalRiskAdministratorId = 0 };
            var modelNegative = CreateValidRequest() with { OccupationalRiskAdministratorId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_OccupationalRiskAdministratorId_Is_Valid()
        {
            var model = CreateValidRequest() with { OccupationalRiskAdministratorId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
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
