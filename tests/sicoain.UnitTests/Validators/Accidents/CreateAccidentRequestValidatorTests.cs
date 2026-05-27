using FluentValidation.TestHelper;
using sicoain.api.Validators.Accidents;
using sicoain.shared.DTOs.Accident;
using Xunit;

namespace sicoain.UnitTests.Validators.Accidents
{
    public class CreateAccidentRequestValidatorTests
    {
        private readonly CreateAccidentRequestValidator _validator = new();

        private CreateAccidentRequest CreateValidRequest() => new()
        {
            EventDate = DateTime.Today,
            Description = new string('A', 20), // length between 10 and 500
            EmployeeId = 1,
            AccidentTypeId = 1,
            EventCategoryId = 1
        };

        [Fact]
        public void Should_Have_Error_When_EventDate_Is_Empty()
        {
            // DateTime cannot be null, but we can test a default value? Not needed.
            // This test is trivial because DateTime is non-nullable.
            // Instead test future date.
            var model = CreateValidRequest() with { EventDate = DateTime.Today.AddDays(1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EventDate);
        }

        [Fact]
        public void Should_Have_Error_When_EventDate_Is_Default()
        {
            // default DateTime (0001-01-01) should trigger NotEmpty
            var model = CreateValidRequest() with { EventDate = default };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EventDate);
        }

        [Fact]
        public void Should_Have_Error_When_EventDate_Is_Future()
        {
            var model = CreateValidRequest() with { EventDate = DateTime.Today.AddDays(1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EventDate);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EventDate_Is_Today_Or_Past()
        {
            var modelToday = CreateValidRequest() with { EventDate = DateTime.Today };
            var modelPast = CreateValidRequest() with { EventDate = DateTime.Today.AddDays(-1) };
            _validator.TestValidate(modelToday).ShouldNotHaveValidationErrorFor(x => x.EventDate);
            _validator.TestValidate(modelPast).ShouldNotHaveValidationErrorFor(x => x.EventDate);
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
            var model = CreateValidRequest() with { Description = new string('A', 9) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Too_Long()
        {
            var model = CreateValidRequest() with { Description = new string('A', 501) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Valid_Length()
        {
            var model = CreateValidRequest() with { Description = new string('A', 50) };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_EmployeeId_Is_Invalid(int invalidId)
        {
            var model = CreateValidRequest() with { EmployeeId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EmployeeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EmployeeId_Is_Valid()
        {
            var model = CreateValidRequest() with { EmployeeId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EmployeeId);
        }

        // Similar tests for AccidentTypeId and EventCategoryId (omitted for brevity, follow same pattern)
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_AccidentTypeId_Is_Invalid(int invalidId)
        {
            var model = CreateValidRequest() with { AccidentTypeId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AccidentTypeId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AccidentTypeId_Is_Valid()
        {
            var model = CreateValidRequest() with { AccidentTypeId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AccidentTypeId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_EventCategoryId_Is_Invalid(int invalidId)
        {
            var model = CreateValidRequest() with { EventCategoryId = invalidId };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EventCategoryId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_EventCategoryId_Is_Valid()
        {
            var model = CreateValidRequest() with { EventCategoryId = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EventCategoryId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var model = CreateValidRequest();
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
