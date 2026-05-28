using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.HealthPromotionEntities;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
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

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HealthPromotionEntity, HealthPromotionEntityDto>();
                cfg.CreateMap<CreateHealthPromotionEntityRequest, HealthPromotionEntity>();
                cfg.CreateMap<UpdateHealthPromotionEntityRequest, HealthPromotionEntity>();
            });
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
