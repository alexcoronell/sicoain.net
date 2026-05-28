using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.CorrectiveActions;
using sicoain.shared.Entities;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class CorrectiveActionServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly CorrectiveActionService _service;

        public CorrectiveActionServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrectiveAction, CorrectiveActionDto>();
                cfg.CreateMap<CreateCorrectiveActionRequest, CorrectiveAction>();
                cfg.CreateMap<UpdateCorrectiveActionRequest, CorrectiveAction>();
            });
            _mapper = config.CreateMapper();

            _service = new CorrectiveActionService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedActions_WithAccidentDescription()
        {
            // Arrange
            var accidentType = new AccidentType { Id = 1, Name = "Type" };
            var eventCategory = new EventCategory { Id = 1, Name = "Category", LevelOfSeverity = AccidentSeverity.Mild };
            var accidentEmployee = new Employee
            {
                Id = 1,
                FirstName = "Worker",
                Surname = "One",
                DocumentType = DocumentType.CedulaDeCiudadania,
                DocumentNumber = "DOC-001",
                SecondName = "",
                SecondSurname = "",
                State = "State",
                Municipality = "City",
                Neighborhood = "Area",
                AddressStreet = "123 St",
                PostalCode = "00000",
                HiringDate = DateTime.Today,
                BusinessId = 1,
                BranchId = 1,
                HealthPromotionEntityId = 1,
                OccupationalRiskAdministratorId = 1,
                DepartmentId = 1,
                PositionId = 1
            };
            var accident = new Accident
            {
                Id = 1,
                EventDate = DateTime.Today,
                Description = "Fall accident",
                Employee = accidentEmployee,
                EmployeeId = accidentEmployee.Id,
                AccidentType = accidentType,
                AccidentTypeId = accidentType.Id,
                EventCategory = eventCategory,
                EventCategoryId = eventCategory.Id
            };
            _context.AccidentTypes.Add(accidentType);
            _context.EventCategories.Add(eventCategory);
            _context.Employees.Add(accidentEmployee);
            _context.Accidents.Add(accident);
            _context.CorrectiveActions.Add(new CorrectiveAction
            {
                Id = 10,
                Title = "Fix floor",
                Description = "Fix the floor",
                Status = StatusAction.Proposal,
                Priority = Priority.Medium,
                Accident = accident,
                AccidentId = accident.Id
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            var dto = result.Items.First();
            dto.Title.Should().Be("Fix floor");
        }

        [Fact]
        public async Task GetByIdAsync_WhenActionExists_ReturnsActionDto()
        {
            // Arrange
            var accidentType = new AccidentType { Id = 2, Name = "Type2" };
            var eventCategory = new EventCategory { Id = 2, Name = "Category2", LevelOfSeverity = AccidentSeverity.Mild };
            var accidentEmployee = new Employee
            {
                Id = 2,
                FirstName = "Worker",
                Surname = "Two",
                DocumentType = DocumentType.CedulaDeCiudadania,
                DocumentNumber = "DOC-002",
                SecondName = "",
                SecondSurname = "",
                State = "State",
                Municipality = "City",
                Neighborhood = "Area",
                AddressStreet = "123 St",
                PostalCode = "00000",
                HiringDate = DateTime.Today,
                BusinessId = 1,
                BranchId = 1,
                HealthPromotionEntityId = 1,
                OccupationalRiskAdministratorId = 1,
                DepartmentId = 1,
                PositionId = 1
            };
            var accident = new Accident
            {
                Id = 2,
                EventDate = DateTime.Today,
                Description = "Chemical spill",
                Employee = accidentEmployee,
                EmployeeId = accidentEmployee.Id,
                AccidentType = accidentType,
                AccidentTypeId = accidentType.Id,
                EventCategory = eventCategory,
                EventCategoryId = eventCategory.Id
            };
            _context.AccidentTypes.Add(accidentType);
            _context.EventCategories.Add(eventCategory);
            _context.Employees.Add(accidentEmployee);
            _context.Accidents.Add(accident);
            var action = new CorrectiveAction
            {
                Id = 20,
                Title = "Clean up",
                Description = "Clean up the spill",
                Status = StatusAction.Proposal,
                Priority = Priority.High,
                Accident = accident,
                AccidentId = accident.Id
            };
            _context.CorrectiveActions.Add(action);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(20);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("Clean up");
        }

        [Fact]
        public async Task GetByIdAsync_WhenActionNotFound_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }
    }
}
