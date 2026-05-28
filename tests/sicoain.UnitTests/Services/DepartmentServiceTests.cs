using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Departments;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class DepartmentServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Department, DepartmentDto>();
                cfg.CreateMap<CreateDepartmentRequest, Department>();
                cfg.CreateMap<UpdateDepartmentRequest, Department>();
            });
            _mapper = config.CreateMapper();

            _service = new DepartmentService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedDepartments()
        {
            // Arrange
            _context.Departments.Add(new Department { Id = 1, Name = "HR" });
            _context.Departments.Add(new Department { Id = 2, Name = "IT" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Select(d => d.Name).Should().Contain("HR");
        }

        [Fact]
        public async Task GetByIdAsync_WhenDepartmentExists_ReturnsDepartmentDto()
        {
            // Arrange
            var dept = new Department { Id = 5, Name = "Finance" };
            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Finance");
        }

        [Fact]
        public async Task GetByIdAsync_WhenDepartmentNotFound_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }
    }
}
