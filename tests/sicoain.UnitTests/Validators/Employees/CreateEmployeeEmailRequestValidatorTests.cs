using FluentValidation.TestHelper;
using sicoain.api.Validators.Employees;
using sicoain.shared.DTOs.Employees;
using Xunit;

namespace sicoain.UnitTests.Validators.Employees
{
    public class CreateEmployeeEmailRequestValidatorTests
    {
        private readonly CreateEmployeeEmailRequestValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Email_IsEmpty()
        {
            var model = new CreateEmployeeEmailRequest { Email = "", EmployeeId = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_IsInvalid()
        {
            var model = new CreateEmployeeEmailRequest { Email = "not-an-email", EmployeeId = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_EmployeeId_IsZeroOrNegative(int employeeId)
        {
            var model = new CreateEmployeeEmailRequest { Email = "test@example.com", EmployeeId = employeeId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid()
        {
            var model = new CreateEmployeeEmailRequest { Email = "test@example.com", EmployeeId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
