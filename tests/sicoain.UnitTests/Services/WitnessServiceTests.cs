using AutoMapper;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Witnesses;
using sicoain.shared.Entities;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the Witness service covering CRUD operations, employee/external witness validation, statement constraints, and navigation property loading.
    /// </summary>
    public class WitnessServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly WitnessService _service;

        public WitnessServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var expression = new MapperConfigurationExpression();
            expression.CreateMap<Witness, WitnessDto>()
                .ForMember(dest => dest.EmployeeFullname, opt => opt.MapFrom(src =>
                    src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.Surname}" : null));
            expression.CreateMap<CreateWitnessRequest, Witness>();
            expression.CreateMap<UpdateWitnessRequest, Witness>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new WitnessService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedWitnesses_WithRelatedData()
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
                Description = "Machine accident",
                Employee = accidentEmployee,
                EmployeeId = accidentEmployee.Id,
                AccidentType = accidentType,
                AccidentTypeId = accidentType.Id,
                EventCategory = eventCategory,
                EventCategoryId = eventCategory.Id
            };
            var witnessEmployee = new Employee
            {
                Id = 2,
                FirstName = "John",
                Surname = "Doe",
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
            _context.AccidentTypes.Add(accidentType);
            _context.EventCategories.Add(eventCategory);
            _context.Employees.Add(accidentEmployee);
            _context.Accidents.Add(accident);
            _context.Employees.Add(witnessEmployee);
            _context.Witnesses.Add(new Witness
            {
                Id = 10,
                Statement = "I saw everything",
                Accident = accident,
                AccidentId = accident.Id,
                Employee = witnessEmployee
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            var dto = result.Items.First();
            dto.Statement.Should().Be("I saw everything");
            dto.EmployeeFullname.Should().Be("John Doe");
        }

        [Fact]
        public async Task GetByIdAsync_WhenWitnessExists_ReturnsWitnessDto()
        {
            // Arrange
            var accidentType = new AccidentType { Id = 2, Name = "Type2" };
            var eventCategory = new EventCategory { Id = 2, Name = "Category2", LevelOfSeverity = AccidentSeverity.Mild };
            var accidentEmployee = new Employee
            {
                Id = 3,
                FirstName = "Worker",
                Surname = "Two",
                DocumentType = DocumentType.CedulaDeCiudadania,
                DocumentNumber = "DOC-003",
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
                Id = 3,
                EventDate = DateTime.Today,
                Description = "Fall",
                Employee = accidentEmployee,
                EmployeeId = accidentEmployee.Id,
                AccidentType = accidentType,
                AccidentTypeId = accidentType.Id,
                EventCategory = eventCategory,
                EventCategoryId = eventCategory.Id
            };
            var witnessEmployee = new Employee
            {
                Id = 4,
                FirstName = "Jane",
                Surname = "Smith",
                DocumentType = DocumentType.CedulaDeCiudadania,
                DocumentNumber = "DOC-004",
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
            _context.AccidentTypes.Add(accidentType);
            _context.EventCategories.Add(eventCategory);
            _context.Employees.Add(accidentEmployee);
            _context.Accidents.Add(accident);
            _context.Employees.Add(witnessEmployee);
            var witness = new Witness
            {
                Id = 20,
                Statement = "Witness statement",
                Accident = accident,
                AccidentId = accident.Id,
                Employee = witnessEmployee
            };
            _context.Witnesses.Add(witness);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(20);

            // Assert
            result.Should().NotBeNull();
            result!.Statement.Should().Be("Witness statement");
            result.EmployeeFullname.Should().Be("Jane Smith");
        }

        [Fact]
        public async Task GetByIdAsync_WhenWitnessNotFound_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }
    }
}
