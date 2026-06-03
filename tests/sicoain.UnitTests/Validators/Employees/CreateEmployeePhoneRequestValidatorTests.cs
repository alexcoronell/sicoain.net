using FluentValidation.TestHelper;
using sicoain.api.Validators.Employees;
using sicoain.shared.DTOs.Employees;
using Xunit;

namespace sicoain.UnitTests.Validators.Employees
{
    /// <summary>
    /// Unit tests for the CreateEmployeePhoneRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateEmployeePhoneRequestValidatorTests
    {
        private readonly CreateEmployeePhoneRequestValidator _validator = new();

        [Theory]
        [InlineData("")]           // empty
        [InlineData("123")]        // too short
        [InlineData("123456789012345")] // too long (15 digits)
        public void Should_Have_Error_When_Phone_IsInvalid(string phone)
        {
            var model = new CreateEmployeePhoneRequest { Phone = phone, EmployeeId = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_EmployeeId_IsZeroOrNegative(int employeeId)
        {
            var model = new CreateEmployeePhoneRequest { Phone = "3101234567", EmployeeId = employeeId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Theory]
        [InlineData("3101234567")]
        [InlineData("+573101234567")]
        [InlineData("573101234567")]
        [InlineData("6012345678")]
        public void Should_Not_Have_Error_When_Phone_IsValid(string phone)
        {
            var model = new CreateEmployeePhoneRequest { Phone = phone, EmployeeId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
