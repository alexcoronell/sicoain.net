using FluentValidation.TestHelper;
using sicoain.api.Validators;
using sicoain.shared.DTOs.Branches;
using Xunit;

namespace sicoain.UnitTests.Validators.Branches
{
    public class UpdateBranchRequestValidatorTests
    {
        private readonly UpdateBranchRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateBranchRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateBranchRequest { Name = null };
            var modelEmpty = new UpdateBranchRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdateBranchRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdateBranchRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdateBranchRequest { Name = "Updated Branch" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_Is_Provided_And_Too_Long()
        {
            var model = new UpdateBranchRequest { AddressStreet = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AddressStreet_Is_Provided_And_Valid()
        {
            var model = new UpdateBranchRequest { AddressStreet = "123 New Address" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_BusinessId_Is_Not_Provided()
        {
            var model = new UpdateBranchRequest { BusinessId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.BusinessId);
        }

        [Fact]
        public void Should_Have_Error_When_BusinessId_Is_Provided_And_ZeroOrNegative()
        {
            var modelZero = new UpdateBranchRequest { BusinessId = 0 };
            var modelNegative = new UpdateBranchRequest { BusinessId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.BusinessId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.BusinessId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_BusinessId_Is_Provided_And_Valid()
        {
            var model = new UpdateBranchRequest { BusinessId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.BusinessId);
        }
    }
}
