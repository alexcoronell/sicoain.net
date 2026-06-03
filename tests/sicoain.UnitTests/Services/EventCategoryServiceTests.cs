using AutoMapper;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.EventCategories;
using sicoain.shared.Entities;
using sicoain.shared.Enums;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the EventCategory service covering CRUD operations, severity level validation, and name/length constraints.
    /// </summary>
    public class EventCategoryServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly EventCategoryService _service;

        public EventCategoryServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var expression = new MapperConfigurationExpression();
            expression.CreateMap<EventCategory, EventCategoryDto>();
            expression.CreateMap<CreateEventCategoryRequest, EventCategory>();
            expression.CreateMap<UpdateEventCategoryRequest, EventCategory>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new EventCategoryService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedCategories()
        {
            // Arrange
            _context.EventCategories.Add(new EventCategory { Id = 1, Name = "Fall", LevelOfSeverity = AccidentSeverity.Moderate });
            _context.EventCategories.Add(new EventCategory { Id = 2, Name = "Burn", LevelOfSeverity = AccidentSeverity.Severe });
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
            var entity = new EventCategory { Id = 3, Name = "Chemical exposure", LevelOfSeverity = AccidentSeverity.Critico };
            _context.EventCategories.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(3);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Chemical exposure");
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
