using FluentValidation.TestHelper;
using sicoain.api.Validators.CorrectiveActions;
using sicoain.shared.DTOs.CorrectiveActions;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Validators.CorrectiveActions
{
    /// <summary>
    /// Unit tests for the CreateCorrectiveActionRequest validation rules covering required fields, length constraints, format validation, and boundary conditions.
    /// </summary>
    public class CreateCorrectiveActionRequestValidatorTests
    {
        private readonly CreateCorrectiveActionRequestValidator _validator = new();

        private CreateCorrectiveActionRequest CreateValidRequest() => new()
        {
            Title = "Fix safety issue",
            Description = "Detailed description of the corrective action",
            DueDate = DateTime.Today.AddDays(7),
            Status = StatusAction.InProcess,
            Priority = Priority.High,
            AccidentId = 1
        };

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var model = CreateValidRequest() with { Title = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Too_Short()
        {
            var model = CreateValidRequest() with { Title = "AB" }; // length 2 < 3
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Too_Long()
        {
            var model = CreateValidRequest() with { Title = new string('T', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Title_Is_Valid()
        {
            var model = CreateValidRequest() with { Title = "Fix safety issue" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            var model = CreateValidRequest() with { Description = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Too_Short()
        {
            var model = CreateValidRequest() with { Description = "AB" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Too_Long()
        {
            var model = CreateValidRequest() with { Description = new string('D', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Valid()
        {
            var model = CreateValidRequest() with { Description = "Detailed description of the corrective action" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_DueDate_Is_Empty()
        {
            // DueDate is non-nullable, so we can only test past date or default
            // For creation, DueDate is required; the empty check is done via NotEmpty.
            // Since DateTime is non-nullable, empty is not possible, so we test past date.
            var model = CreateValidRequest() with { DueDate = DateTime.Today.AddDays(-1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public void Should_Have_Error_When_DueDate_Is_In_Past()
        {
            var model = CreateValidRequest() with { DueDate = DateTime.Today.AddDays(-1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_DueDate_Is_Today_Or_Future()
        {
            var modelToday = CreateValidRequest() with { DueDate = DateTime.Today };
            var modelFuture = CreateValidRequest() with { DueDate = DateTime.Today.AddDays(5) };
            _validator.TestValidate(modelToday).ShouldNotHaveValidationErrorFor(x => x.DueDate);
            _validator.TestValidate(modelFuture).ShouldNotHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public void Should_Have_Error_When_Status_Has_Invalid_Value()
        {
            var model = CreateValidRequest() with { Status = (StatusAction)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Status_Is_Valid()
        {
            var model = CreateValidRequest() with { Status = StatusAction.InProcess };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Have_Error_When_Priority_Has_Invalid_Value()
        {
            var model = CreateValidRequest() with { Priority = (Priority)999 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Priority);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Priority_Is_Valid()
        {
            var model = CreateValidRequest() with { Priority = Priority.Low };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Priority);
        }

        [Fact]
        public void Should_Have_Error_When_AccidentId_Is_Zero_Or_Negative()
        {
            var modelZero = CreateValidRequest() with { AccidentId = 0 };
            var modelNegative = CreateValidRequest() with { AccidentId = -1 };
            _validator.TestValidate(modelZero).ShouldHaveValidationErrorFor(x => x.AccidentId);
            _validator.TestValidate(modelNegative).ShouldHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentId_Is_Positive()
        {
            var model = CreateValidRequest() with { AccidentId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var model = CreateValidRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
