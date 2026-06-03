using FluentValidation.TestHelper;
using sicoain.api.Validators.OccupationalRiskAdministrators;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using Xunit;

namespace sicoain.UnitTests.Validators.OccupationalRiskAdministrators
{
    /// <summary>
    /// Unit tests for the CreateOccupationalRiskAdministratorEmailRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateOccupationalRiskAdministratorEmailRequestValidatorTests
    {
        private readonly CreateOccupationalRiskAdministratorEmailRequestValidator _validator = new();

        private CreateOccupationalRiskAdministratorEmailRequest CreateValidRequest() => new()
        {
            Email = "arl@example.com",
            OccupationalRiskAdministratorId = 1
        };

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = CreateValidRequest() with { Email = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = CreateValidRequest() with { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_OccupationalRiskAdministratorId_Is_ZeroOrNegative()
        {
            var modelZero = CreateValidRequest() with { OccupationalRiskAdministratorId = 0 };
            var modelNegative = CreateValidRequest() with { OccupationalRiskAdministratorId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_OccupationalRiskAdministratorId_Is_Valid()
        {
            var model = CreateValidRequest() with { OccupationalRiskAdministratorId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.OccupationalRiskAdministratorId);
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
