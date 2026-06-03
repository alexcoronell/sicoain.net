using FluentValidation.TestHelper;
using sicoain.api.Validators;
using sicoain.shared.DTOs.Witnesses;
using Xunit;

namespace sicoain.UnitTests.Validators.Witnesses
{
    /// <summary>
    /// Unit tests for the CreateWitnessRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateWitnessRequestValidatorTests
    {
        private readonly CreateWitnessRequestValidator _validator = new();

        private CreateWitnessRequest CreateValidInternalWitness() => new()
        {
            AccidentId = 1,
            EmployeeId = 5,
            WitnessName = null,
            Statement = new string('A', 100) // length between 50 and 500
        };

        private CreateWitnessRequest CreateValidExternalWitness() => new()
        {
            AccidentId = 1,
            EmployeeId = null,
            WitnessName = "John Doe",
            Statement = new string('A', 100)
        };

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_AccidentId_Is_Invalid(int invalidId)
        {
            var model = CreateValidInternalWitness() with { AccidentId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentId_Is_Valid()
        {
            var model = CreateValidInternalWitness() with { AccidentId = 10 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Have_Error_When_Neither_EmployeeId_Nor_WitnessName_Is_Provided()
        {
            var model = CreateValidInternalWitness() with { EmployeeId = null, WitnessName = null };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Fact]
        public void Should_Have_Error_When_Both_EmployeeId_And_WitnessName_Are_Provided()
        {
            var model = CreateValidInternalWitness() with { EmployeeId = 5, WitnessName = "John" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_EmployeeId_Is_Invalid(int invalidId)
        {
            var model = CreateValidInternalWitness() with { EmployeeId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EmployeeId_Is_Valid()
        {
            var model = CreateValidInternalWitness() with { EmployeeId = 10 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Fact]
        public void Should_Have_Error_When_EmployeeId_Is_Provided_And_WitnessName_Is_Also_Provided()
        {
            var model = CreateValidInternalWitness() with { WitnessName = "Jane" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Fact]
        public void Should_Have_Error_When_WitnessName_Exceeds_MaxLength()
        {
            var model = CreateValidExternalWitness() with { WitnessName = new string('W', 151) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.WitnessName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_WitnessName_Is_Valid()
        {
            var model = CreateValidExternalWitness() with { WitnessName = "Jane Doe" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.WitnessName);
        }

        [Fact]
        public void Should_Have_Error_When_WitnessContact_Exceeds_MaxLength()
        {
            var model = CreateValidExternalWitness() with { WitnessContact = new string('C', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.WitnessContact);
        }

        [Fact]
        public void Should_Not_Have_Error_When_WitnessContact_Is_Valid()
        {
            var model = CreateValidExternalWitness() with { WitnessContact = "+573001234567" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.WitnessContact);
        }

        [Fact]
        public void Should_Have_Error_When_Statement_Is_Empty()
        {
            var model = CreateValidInternalWitness() with { Statement = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Statement);
        }

        [Fact]
        public void Should_Have_Error_When_Statement_Is_Too_Short()
        {
            var model = CreateValidInternalWitness() with { Statement = new string('A', 49) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Statement);
        }

        [Fact]
        public void Should_Have_Error_When_Statement_Is_Too_Long()
        {
            var model = CreateValidInternalWitness() with { Statement = new string('A', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Statement);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Statement_Is_Valid()
        {
            var model = CreateValidInternalWitness() with { Statement = new string('A', 100) };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Statement);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Internal_Witness_Is_Valid()
        {
            var model = CreateValidInternalWitness();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_External_Witness_Is_Valid()
        {
            var model = CreateValidExternalWitness();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
