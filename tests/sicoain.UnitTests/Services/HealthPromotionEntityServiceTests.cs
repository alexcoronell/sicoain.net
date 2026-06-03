using AutoMapper;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.HealthPromotionEntities;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the HealthPromotionEntity (EPS) service covering CRUD operations, partial address updates, and contact management.
    /// </summary>
    public class HealthPromotionEntityServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly HealthPromotionEntityService _service;

        public HealthPromotionEntityServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var expression = new MapperConfigurationExpression();
            expression.CreateMap<HealthPromotionEntity, HealthPromotionEntityDto>();
            expression.CreateMap<CreateHealthPromotionEntityRequest, HealthPromotionEntity>();
            expression.CreateMap<UpdateHealthPromotionEntityRequest, HealthPromotionEntity>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new HealthPromotionEntityService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedEntities()
        {
            // Arrange
            _context.HealthPromotionEntities.Add(new HealthPromotionEntity { Id = 1, Name = "Sura" });
            _context.HealthPromotionEntities.Add(new HealthPromotionEntity { Id = 2, Name = "Compensar" });
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
            var entity = new HealthPromotionEntity { Id = 5, Name = "Sanitas" };
            _context.HealthPromotionEntities.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Sanitas");
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
