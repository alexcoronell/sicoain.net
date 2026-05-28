using FluentValidation.TestHelper;
using sicoain.api.Validators.CorrectiveActions;
using sicoain.shared.DTOs.CorrectiveActions;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.CorrectiveActions
{
    public class UpdateCorrectiveActionRequestValidatorTests
    {
        private readonly UpdateCorrectiveActionRequestValidator _validator = new();

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Null()
        {
            var model = new UpdateCorrectiveActionRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Title_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateCorrectiveActionRequest { Title = null };
            var modelEmpty = new UpdateCorrectiveActionRequest { Title = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Title);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Provided_And_Too_Short()
        {
            var model = new UpdateCorrectiveActionRequest { Title = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Provided_And_Too_Long()
        {
            var model = new UpdateCorrectiveActionRequest { Title = new string('T', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Title_Is_Provided_And_Valid()
        {
            var model = new UpdateCorrectiveActionRequest { Title = "Updated title" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null_Or_Empty()
        {
            var modelNull = new UpdateCorrectiveActionRequest { Description = null };
            var modelEmpty = new UpdateCorrectiveActionRequest { Description = "" };
            _validator.TestValidate(modelNull).ShouldNotHaveValidationErrorFor(x => x.Description);
            _validator.TestValidate(modelEmpty).ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Short()
        {
            var model = new UpdateCorrectiveActionRequest { Description = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Provided_And_Too_Long()
        {
            var model = new UpdateCorrectiveActionRequest { Description = new string('D', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Provided_And_Valid()
        {
            var model = new UpdateCorrectiveActionRequest { Description = "Updated description" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DueDate_Is_Not_Provided()
        {
            var model = new UpdateCorrectiveActionRequest { DueDate = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public void Should_Have_Error_When_DueDate_Is_Provided_And_In_Past()
        {
            var model = new UpdateCorrectiveActionRequest { DueDate = DateTime.Today.AddDays(-1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DueDate_Is_Provided_And_Valid()
        {
            var modelToday = new UpdateCorrectiveActionRequest { DueDate = DateTime.Today };
            var modelFuture = new UpdateCorrectiveActionRequest { DueDate = DateTime.Today.AddDays(3) };
            _validator.TestValidate(modelToday).ShouldNotHaveValidationErrorFor(x => x.DueDate);
            _validator.TestValidate(modelFuture).ShouldNotHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Status_Is_Not_Provided()
        {
            var model = new UpdateCorrectiveActionRequest { Status = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Have_Error_When_Status_Is_Provided_And_Invalid()
        {
            var model = new UpdateCorrectiveActionRequest { Status = (StatusAction)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Status_Is_Provided_And_Valid()
        {
            var model = new UpdateCorrectiveActionRequest { Status = StatusAction.Completed };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Priority_Is_Not_Provided()
        {
            var model = new UpdateCorrectiveActionRequest { Priority = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Priority);
        }

        [Fact]
        public void Should_Have_Error_When_Priority_Is_Provided_And_Invalid()
        {
            var model = new UpdateCorrectiveActionRequest { Priority = (Priority)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Priority);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Priority_Is_Provided_And_Valid()
        {
            var model = new UpdateCorrectiveActionRequest { Priority = Priority.Medium };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Priority);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentId_Is_Not_Provided()
        {
            var model = new UpdateCorrectiveActionRequest { AccidentId = null };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Have_Error_When_AccidentId_Is_Provided_And_ZeroOrNegative()
        {
            var modelZero = new UpdateCorrectiveActionRequest { AccidentId = 0 };
            var modelNegative = new UpdateCorrectiveActionRequest { AccidentId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.AccidentId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentId_Is_Provided_And_Positive()
        {
            var model = new UpdateCorrectiveActionRequest { AccidentId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentId);
        }
    }
}
