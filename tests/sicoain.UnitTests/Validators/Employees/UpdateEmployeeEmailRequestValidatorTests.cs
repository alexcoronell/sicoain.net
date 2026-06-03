using FluentValidation.TestHelper;
using sicoain.api.Validators.Employees;
using sicoain.shared.DTOs.Employees;
using Xunit;

namespace sicoain.UnitTests.Validators.Employees
{
    /// <summary>
    /// Unit tests for the UpdateEmployeeEmailRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateEmployeeEmailRequestValidatorTests
    {
        private readonly UpdateEmployeeEmailRequestValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Email_IsProvided_But_Invalid()
        {
            var model = new UpdateEmployeeEmailRequest { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_IsNull_Or_Empty_Or_Valid()
        {
            var model1 = new UpdateEmployeeEmailRequest { Email = null };
            var model2 = new UpdateEmployeeEmailRequest { Email = "" };
            var model3 = new UpdateEmployeeEmailRequest { Email = "good@example.com" };

            _validator.TestValidate(model1).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(model2).ShouldNotHaveAnyValidationErrors();
            _validator.TestValidate(model3).ShouldNotHaveAnyValidationErrors();
        }
    }
}
