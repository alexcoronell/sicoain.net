using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class OccupationalRiskAdministratorServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly OccupationalRiskAdministratorService _service;

        public OccupationalRiskAdministratorServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OccupationalRiskAdministrator, OccupationalRiskAdministratorDto>();
                cfg.CreateMap<CreateOccupationalRiskAdministratorRequest, OccupationalRiskAdministrator>();
                cfg.CreateMap<UpdateOccupationalRiskAdministratorRequest, OccupationalRiskAdministrator>();
            });
            _mapper = config.CreateMapper();

            _service = new OccupationalRiskAdministratorService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedOras()
        {
            // Arrange
            _context.OccupationalRiskAdministrators.Add(new OccupationalRiskAdministrator { Id = 1, Name = "ARL Sura" });
            _context.OccupationalRiskAdministrators.Add(new OccupationalRiskAdministrator { Id = 2, Name = "ARL Positiva" });
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
            var entity = new OccupationalRiskAdministrator { Id = 4, Name = "ARL Colpatria" };
            _context.OccupationalRiskAdministrators.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(4);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("ARL Colpatria");
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
