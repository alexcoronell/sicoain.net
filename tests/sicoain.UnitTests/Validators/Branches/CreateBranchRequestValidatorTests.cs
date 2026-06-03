using FluentValidation.TestHelper;
using sicoain.api.Validators;
using sicoain.shared.DTOs.Branches;
using Xunit;

namespace sicoain.UnitTests.Validators.Branches
{
    /// <summary>
    /// Unit tests for the CreateBranchRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateBranchRequestValidatorTests
    {
        private readonly CreateBranchRequestValidator _validator = new();

        private CreateBranchRequest CreateValidRequest() => new()
        {
            Name = "Main Branch",
            AddressStreet = "123 Main St",
            BusinessId = 1
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
            var model = CreateValidRequest() with { Name = "Valid Branch" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_Exceeds_MaxLength()
        {
            var model = CreateValidRequest() with { AddressStreet = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AddressStreet_Is_Null_Or_Within_Limit()
        {
            var modelNull = CreateValidRequest() with { AddressStreet = null };
            var modelValid = CreateValidRequest() with { AddressStreet = new string('A', 200) };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
            _validator.TestValidate(modelValid).ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Have_Error_When_BusinessId_Is_Null()
        {
            var model = CreateValidRequest() with { BusinessId = null };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.BusinessId);
        }

        [Fact]
        public void Should_Have_Error_When_BusinessId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { BusinessId = 0 };
            var modelNegative = CreateValidRequest() with { BusinessId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.BusinessId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.BusinessId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_BusinessId_Is_Valid()
        {
            var model = CreateValidRequest() with { BusinessId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.BusinessId);
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
