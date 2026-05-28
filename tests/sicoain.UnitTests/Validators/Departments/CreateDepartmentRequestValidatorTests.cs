using FluentValidation.TestHelper;
using sicoain.api.Validators.Departments;
using sicoain.shared.DTOs.Departments;
using Xunit;

namespace sicoain.UnitTests.Validators.Departments
{
    public class CreateDepartmentRequestValidatorTests
    {
        private readonly CreateDepartmentRequestValidator _validator = new();

        private CreateDepartmentRequest CreateValidRequest() => new()
        {
            Name = "Human Resources",
            Description = "Manages personnel",
            Email = "hr@company.com",
            Phone = "3101234567"
        };

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = CreateValidRequest() with { Name = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            var model = CreateValidRequest() with { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Long()
        {
            var model = CreateValidRequest() with { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Valid()
        {
            var model = CreateValidRequest() with { Name = "Finance" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { Description = new string('D', 251) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null_Or_Within_Limit()
        {
            var modelNull = CreateValidRequest() with { Description = null };
            var modelValid = CreateValidRequest() with { Description = new string('D', 250) };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Description);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = CreateValidRequest() with { Email = "invalid-email" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Null_Or_Valid()
        {
            var modelNull = CreateValidRequest() with { Email = null };
            var modelValid = CreateValidRequest() with { Email = "valid@example.com" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Email);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.Email);
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
        public void Should_Not_Have_Error_When_Phone_Is_Null_Or_Empty()
        {
            var modelNull = CreateValidRequest() with { Phone = null };
            var modelEmpty = CreateValidRequest() with { Phone = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Phone);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Phone);
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
