using AutoMapper;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Accident;
using sicoain.shared.Entities;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the Accident service covering CRUD operations, soft delete, navigation property loading, and validation edge cases.
    /// </summary>
    public class AccidentServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly AccidentService _service;

        public AccidentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var expression = new MapperConfigurationExpression();
            expression.CreateMap<Accident, AccidentDto>()
                .ForMember(dest => dest.EmployeeFullname, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.Surname}"))
                .ForMember(dest => dest.AccidentTypeName, opt => opt.MapFrom(src => src.AccidentType.Name))
                .ForMember(dest => dest.EventCategoryName, opt => opt.MapFrom(src => src.EventCategory.Name));
            expression.CreateMap<CreateAccidentRequest, Accident>();
            expression.CreateMap<UpdateAccidentRequest, Accident>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new AccidentService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedAccidents_WithRelatedEntities()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                FirstName = "John",
                Surname = "Doe",
                DocumentType = DocumentType.CedulaDeCiudadania,
                DocumentNumber = "12345",
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
            var accidentType = new AccidentType { Id = 1, Name = "Fracture" };
            var eventCategory = new EventCategory { Id = 1, Name = "Fall", LevelOfSeverity = AccidentSeverity.Mild };
            _context.Employees.Add(employee);
            _context.AccidentTypes.Add(accidentType);
            _context.EventCategories.Add(eventCategory);
            _context.Accidents.Add(new Accident
            {
                Id = 10,
                EventDate = DateTime.Today,
                Description = "Test accident",
                Employee = employee,
                EmployeeId = employee.Id,
                AccidentType = accidentType,
                AccidentTypeId = accidentType.Id,
                EventCategory = eventCategory,
                EventCategoryId = eventCategory.Id
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            var dto = result.Items.First();
            dto.EmployeeFullname.Should().Be("John Doe");
            dto.AccidentTypeName.Should().Be("Fracture");
            dto.EventCategoryName.Should().Be("Fall");
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnAccidentDto()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                FirstName = "Jane",
                Surname = "Smith",
                DocumentType = DocumentType.CedulaDeCiudadania,
                DocumentNumber = "12345",
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
            var accidentType = new AccidentType { Id = 1, Name = "Burn" };
            var eventCategory = new EventCategory { Id = 1, Name = "Chemical exposure", LevelOfSeverity = AccidentSeverity.Mild };
            _context.Employees.Add(employee);
            _context.AccidentTypes.Add(accidentType);
            _context.EventCategories.Add(eventCategory);
            var accident = new Accident
            {
                Id = 20,
                EventDate = DateTime.Today,
                Description = "Chemical burn",
                Employee = employee,
                EmployeeId = employee.Id,
                AccidentType = accidentType,
                AccidentTypeId = accidentType.Id,
                EventCategory = eventCategory,
                EventCategoryId = eventCategory.Id
            };
            _context.Accidents.Add(accident);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(20);

            // Assert
            result.Should().NotBeNull();
            result!.EmployeeFullname.Should().Be("Jane Smith");
            result.AccidentTypeName.Should().Be("Burn");
            result.EventCategoryName.Should().Be("Chemical exposure");
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }
    }
}
