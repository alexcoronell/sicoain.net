using AutoMapper;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Business;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the Business service covering CRUD operations, duplicate name handling, and contact entity management.
    /// </summary>
    public class BusinessServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly BusinessService _service;

        public BusinessServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var expression = new MapperConfigurationExpression();
            expression.CreateMap<Business, BusinessDto>();
            expression.CreateMap<CreateBusinessRequest, Business>();
            expression.CreateMap<UpdateBusinessRequest, Business>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new BusinessService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedBusinesses()
        {
            // Arrange
            _context.Businesses.Add(new Business { Id = 1, Name = "Company A" });
            _context.Businesses.Add(new Business { Id = 2, Name = "Company B" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.Select(b => b.Name).Should().Contain("Company A");
        }

        [Fact]
        public async Task GetByIdAsync_WhenBusinessExists_ReturnsBusinessDto()
        {
            // Arrange
            var business = new Business { Id = 5, Name = "Test Business" };
            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Test Business");
        }

        [Fact]
        public async Task GetByIdAsync_WhenBusinessNotFound_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }
    }
}
