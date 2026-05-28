using FluentValidation.TestHelper;
using sicoain.api.Validators.OccupationalRiskAdministrators;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using Xunit;

namespace sicoain.UnitTests.Validators.OccupationalRiskAdministrators
{
    public class UpdateOccupationalRiskAdministratorEmailRequestValidatorTests
    {
        private readonly UpdateOccupationalRiskAdministratorEmailRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateOccupationalRiskAdministratorEmailRequest { Email = null };
            var modelEmpty = new UpdateOccupationalRiskAdministratorEmailRequest { Email = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Email);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Provided_But_Invalid()
        {
            var model = new UpdateOccupationalRiskAdministratorEmailRequest { Email = "invalid" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Provided_And_Valid()
        {
            var model = new UpdateOccupationalRiskAdministratorEmailRequest { Email = "good@arl.com" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
