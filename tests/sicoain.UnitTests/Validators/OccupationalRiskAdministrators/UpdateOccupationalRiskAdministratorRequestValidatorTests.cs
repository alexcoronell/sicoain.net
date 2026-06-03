using FluentValidation.TestHelper;
using sicoain.api.Validators.OccupationalRiskAdministrators;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using Xunit;

namespace sicoain.UnitTests.Validators.OccupationalRiskAdministrators
{
    /// <summary>
    /// Unit tests for the UpdateOccupationalRiskAdministratorRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class UpdateOccupationalRiskAdministratorRequestValidatorTests
    {
        private readonly UpdateOccupationalRiskAdministratorRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Null()
        {
            var model = new UpdateOccupationalRiskAdministratorRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateOccupationalRiskAdministratorRequest { Name = null };
            var modelEmpty = new UpdateOccupationalRiskAdministratorRequest { Name = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Name);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Short()
        {
            var model = new UpdateOccupationalRiskAdministratorRequest { Name = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Provided_And_Too_Long()
        {
            var model = new UpdateOccupationalRiskAdministratorRequest { Name = new string('N', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Provided_And_Valid()
        {
            var model = new UpdateOccupationalRiskAdministratorRequest { Name = "Updated ARL" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_AddressStreet_Is_Provided_And_Too_Long()
        {
            var model = new UpdateOccupationalRiskAdministratorRequest { AddressStreet = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AddressStreet);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AddressStreet_Is_Provided_And_Valid()
        {
            var model = new UpdateOccupationalRiskAdministratorRequest { AddressStreet = "Nueva dirección" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AddressStreet);
        }
    }
}
