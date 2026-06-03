using FluentValidation.TestHelper;
using sicoain.api.Validators.Departments;
using sicoain.shared.DTOs.Departments;
using Xunit;

namespace sicoain.UnitTests.Validators.Departments
{
    /// <summary>
    /// Unit tests for the UpdateDepartmentRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateDepartmentRequestValidatorTests
    {
        private readonly UpdateDepartmentRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateDepartmentRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateDepartmentRequest { Name = null };
            var modelEmpty = new UpdateDepartmentRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdateDepartmentRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdateDepartmentRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdateDepartmentRequest { Name = "Updated Dept" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Long()
        {
            var model = new UpdateDepartmentRequest { Description = new string('D', 251) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Provided_And_Valid()
        {
            var model = new UpdateDepartmentRequest { Description = "Short description" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Provided_But_Invalid()
        {
            var model = new UpdateDepartmentRequest { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Provided_And_Valid()
        {
            var model = new UpdateDepartmentRequest { Email = "valid@example.com" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456789012345")]
        [InlineData("abc")]
        public void Should_Have_Error_When_Phone_Is_Provided_But_Invalid(string invalidPhone)
        {
            var model = new UpdateDepartmentRequest { Phone = invalidPhone };
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
            var model = new UpdateDepartmentRequest { Phone = validPhone };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Phone);
        }
    }
}
