using FluentValidation.TestHelper;
using sicoain.api.Validators;
using sicoain.shared.DTOs.Witnesses;
using Xunit;

namespace sicoain.UnitTests.Validators.Witnesses
{
    public class UpdateWitnessRequestValidatorTests
    {
        private readonly UpdateWitnessRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Null()
        {
            var model = new UpdateWitnessRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_AccidentId_Is_Provided_And_Invalid(int invalidId)
        {
            var model = new UpdateWitnessRequest { AccidentId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentId_Is_Provided_And_Valid()
        {
            var model = new UpdateWitnessRequest { AccidentId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_EmployeeId_Is_Provided_And_Invalid(int invalidId)
        {
            var model = new UpdateWitnessRequest { EmployeeId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EmployeeId_Is_Provided_And_Valid()
        {
            var model = new UpdateWitnessRequest { EmployeeId = 3 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Fact]
        public void Should_Have_Error_When_Both_EmployeeId_And_WitnessName_Are_Provided()
        {
            var model = new UpdateWitnessRequest { EmployeeId = 1, WitnessName = "John" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Only_EmployeeId_Is_Provided()
        {
            var model = new UpdateWitnessRequest { EmployeeId = 1 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Only_WitnessName_Is_Provided()
        {
            var model = new UpdateWitnessRequest { WitnessName = "Jane" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_WitnessName_Exceeds_MaxLength()
        {
            var model = new UpdateWitnessRequest { WitnessName = new string('W', 151) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.WitnessName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_WitnessName_Is_Provided_And_Valid()
        {
            var model = new UpdateWitnessRequest { WitnessName = "Jane Doe" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.WitnessName);
        }

        [Fact]
        public void Should_Have_Error_When_WitnessContact_Exceeds_MaxLength()
        {
            var model = new UpdateWitnessRequest { WitnessContact = new string('C', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.WitnessContact);
        }

        [Fact]
        public void Should_Not_Have_Error_When_WitnessContact_Is_Valid()
        {
            var model = new UpdateWitnessRequest { WitnessContact = "+573001234567" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.WitnessContact);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Statement_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateWitnessRequest { Statement = null };
            var modelEmpty = new UpdateWitnessRequest { Statement = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Statement);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Statement);
        }

        [Fact]
        public void Should_Have_Error_When_Statement_Is_Provided_And_Too_Short()
        {
            var model = new UpdateWitnessRequest { Statement = new string('A', 49) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Statement);
        }

        [Fact]
        public void Should_Have_Error_When_Statement_Is_Provided_And_Too_Long()
        {
            var model = new UpdateWitnessRequest { Statement = new string('A', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Statement);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Statement_Is_Provided_And_Valid()
        {
            var model = new UpdateWitnessRequest { Statement = new string('A', 100) };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Statement);
        }
    }
}
