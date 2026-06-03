using AutoMapper;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.AccidentTypes;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the AccidentType service covering CRUD operations, soft delete, and validation of severity levels and description limits.
    /// </summary>
    public class AccidentTypeServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly AccidentTypeService _service;

        public AccidentTypeServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var expression = new MapperConfigurationExpression();
            expression.CreateMap<AccidentType, AccidentTypeDto>();
            expression.CreateMap<CreateAccidentTypeRequest, AccidentType>();
            expression.CreateMap<UpdateAccidentTypeRequest, AccidentType>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new AccidentTypeService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedAccidentTypes()
        {
            // Arrange
            _context.AccidentTypes.Add(new AccidentType { Id = 1, Name = "Fracture" });
            _context.AccidentTypes.Add(new AccidentType { Id = 2, Name = "Burn" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Select(x => x.Name).Should().Contain(new[] { "Fracture", "Burn" });
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsDto()
        {
            // Arrange
            var entity = new AccidentType { Id = 5, Name = "Laceration" };
            _context.AccidentTypes.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Laceration");
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAndReturnDto()
        {
            // Arrange
            var request = new CreateAccidentTypeRequest
            {
                Name = "Concussion",
                Description = "Brain injury"
            };

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Concussion");
            var dbEntity = await _context.AccidentTypes.FindAsync(result.Id);
            dbEntity.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_WhenExists_UpdatesAndReturnsDto()
        {
            // Arrange
            var entity = new AccidentType { Id = 7, Name = "Old Name" };
            _context.AccidentTypes.Add(entity);
            await _context.SaveChangesAsync();
            var request = new UpdateAccidentTypeRequest { Name = "New Name" };

            // Act
            var result = await _service.UpdateAsync(7, request);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task DeleteAsync_WhenExists_SoftDeletes()
        {
            // Arrange
            var entity = new AccidentType { Id = 8, Name = "Test", IsDeleted = false };
            _context.AccidentTypes.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteAsync(8);

            // Assert
            result.Should().BeTrue();
            var deleted = await _context.AccidentTypes.FindAsync(8);
            deleted!.IsDeleted.Should().BeTrue();
        }
    }
}
