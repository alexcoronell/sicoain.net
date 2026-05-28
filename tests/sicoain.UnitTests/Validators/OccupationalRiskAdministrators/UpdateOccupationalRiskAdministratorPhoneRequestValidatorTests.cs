using FluentValidation.TestHelper;
using sicoain.api.Validators.OccupationalRiskAdministrators;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using Xunit;

namespace sicoain.UnitTests.Validators.OccupationalRiskAdministrators
{
    public class UpdateOccupationalRiskAdministratorPhoneRequestValidatorTests
    {
        private readonly UpdateOccupationalRiskAdministratorPhoneRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Phone_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateOccupationalRiskAdministratorPhoneRequest { Phone = null };
            var modelEmpty = new UpdateOccupationalRiskAdministratorPhoneRequest { Phone = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Phone);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456789012345")]
        [InlineData("abc")]
        public void Should_Have_Error_When_Phone_Is_Provided_But_Invalid(string invalidPhone)
        {
            var model = new UpdateOccupationalRiskAdministratorPhoneRequest { Phone = invalidPhone };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData("3101234567")]
        [InlineData("+573101234567")]
        [InlineData("573101234567")]
        [InlineData("6012345678")]
        public void Should_Not_Have_Error_When_Phone_Is_Provided_And_Valid(string validPhone)
        {
            var model = new UpdateOccupationalRiskAdministratorPhoneRequest { Phone = validPhone };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Phone);
        }
    }
}
