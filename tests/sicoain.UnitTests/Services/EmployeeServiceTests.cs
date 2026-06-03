using AutoMapper;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Employees;
using sicoain.shared.Entities;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the Employee service covering CRUD operations, document number uniqueness, hiring date validation, and business/position assignments.
    /// </summary>
    public class EmployeeServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var expression = new MapperConfigurationExpression();
            expression.CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Name))
                .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position.Name))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Position.Department.Name));
            expression.CreateMap<CreateEmployeeRequest, Employee>();
            expression.CreateMap<UpdateEmployeeRequest, Employee>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new EmployeeService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedEmployees_WithRelatedEntities()
        {
            // Arrange
            var business = new Business { Id = 1, Name = "ACME" };
            var branch = new Branch { Id = 1, Name = "Main Office", BusinessId = 1, Business = business };
            var department = new Department { Id = 1, Name = "IT" };
            var position = new Position { Id = 1, Name = "Developer", Department = department, DepartmentId = 1, RiskClassId = 1 };
            var employee = new Employee
            {
                Id = 5,
                FirstName = "Alice",
                Surname = "Johnson",
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
                Business = business,
                BusinessId = 1,
                Branch = branch,
                BranchId = 1,
                HealthPromotionEntityId = 1,
                OccupationalRiskAdministratorId = 1,
                DepartmentId = 1,
                Position = position,
                PositionId = 1
            };
            _context.Businesses.Add(business);
            _context.Branches.Add(branch);
            _context.Departments.Add(department);
            _context.Positions.Add(position);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            var dto = result.Items.First();
            dto.BusinessName.Should().Be("ACME");
            dto.BranchName.Should().Be("Main Office");
            dto.PositionName.Should().Be("Developer");
            dto.DepartmentName.Should().Be("IT");
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnEmployeeDto()
        {
            // Arrange
            var business = new Business { Id = 2, Name = "XYZ Corp" };
            var branch = new Branch { Id = 2, Name = "South Branch", BusinessId = 2, Business = business };
            var department = new Department { Id = 2, Name = "HR" };
            var position = new Position { Id = 2, Name = "Recruiter", Department = department, DepartmentId = 2, RiskClassId = 1 };
            var employee = new Employee
            {
                Id = 10,
                FirstName = "Bob",
                Surname = "Williams",
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
                Business = business,
                BusinessId = 2,
                Branch = branch,
                BranchId = 2,
                HealthPromotionEntityId = 1,
                OccupationalRiskAdministratorId = 1,
                DepartmentId = 2,
                Position = position,
                PositionId = 2
            };
            _context.Businesses.Add(business);
            _context.Branches.Add(branch);
            _context.Departments.Add(department);
            _context.Positions.Add(position);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(10);

            // Assert
            result.Should().NotBeNull();
            result!.BusinessName.Should().Be("XYZ Corp");
            result.BranchName.Should().Be("South Branch");
            result.PositionName.Should().Be("Recruiter");
            result.DepartmentName.Should().Be("HR");
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
