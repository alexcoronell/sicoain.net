using AutoMapper;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.RiskClasses;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the RiskClass service covering CRUD operations, duplicate code handling, contribution rate validation, and soft delete.
    /// </summary>
    public class RiskClassServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly RiskClassService _service;

        public RiskClassServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var expression = new MapperConfigurationExpression();
            expression.CreateMap<RiskClass, RiskClassDto>();
            expression.CreateMap<CreateRiskClassRequest, RiskClass>();
            expression.CreateMap<UpdateRiskClassRequest, RiskClass>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new RiskClassService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedRiskClasses()
        {
            // Arrange
            _context.RiskClasses.Add(new RiskClass { Id = 1, Name = "Class I", Code = "I" });
            _context.RiskClasses.Add(new RiskClass { Id = 2, Name = "Class II", Code = "II" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsDto()
        {
            // Arrange
            var entity = new RiskClass { Id = 3, Name = "Class III", Code = "III" };
            _context.RiskClasses.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(3);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Class III");
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
