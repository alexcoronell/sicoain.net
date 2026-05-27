using FluentValidation.TestHelper;
using sicoain.api.Validators.Employees;
using sicoain.shared.DTOs.Employees;
using Xunit;

namespace sicoain.UnitTests.Validators.Employees
{
    public class UpdateEmployeePhoneRequestValidatorTests
    {
        private readonly UpdateEmployeePhoneRequestValidator _validator = new();

        [Theory]
        [InlineData("123")]
        [InlineData("abc")]
        [InlineData("310123456789")] // too long
        public void Should_Have_Error_When_Phone_IsProvided_But_Invalid(string phone)
        {
            var model = new UpdateEmployeePhoneRequest { Phone = phone };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("3101234567")]
        [InlineData("+573101234567")]
        [InlineData("573101234567")]
        [InlineData("6012345678")]
        public void Should_Not_Have_Error_When_Phone_IsNullOrEmpty_Or_Valid(string? phone)
        {
            var model = new UpdateEmployeePhoneRequest { Phone = phone };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
