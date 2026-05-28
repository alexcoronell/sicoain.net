using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Positions;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class PositionServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly PositionService _service;

        public PositionServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Position, PositionDto>();
                cfg.CreateMap<CreatePositionRequest, Position>();
                cfg.CreateMap<UpdatePositionRequest, Position>();
            });
            _mapper = config.CreateMapper();

            _service = new PositionService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldIncludeDepartmentName()
        {
            // Arrange
            var department = new Department { Id = 1, Name = "IT" };
            _context.Departments.Add(department);
            _context.Positions.Add(new Position { Id = 5, Name = "Developer", Department = department, DepartmentId = 1, RiskClassId = 1 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.First().Name.Should().Be("Developer");
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_IncludesDepartmentName()
        {
            // Arrange
            var department = new Department { Id = 2, Name = "HR" };
            _context.Departments.Add(department);
            var position = new Position { Id = 8, Name = "Recruiter", Department = department, DepartmentId = 2, RiskClassId = 1 };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(8);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Recruiter");
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }
    }
}
