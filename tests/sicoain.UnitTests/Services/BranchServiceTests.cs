using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Branches;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class BranchServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly BranchService _service;

        public BranchServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Branch, BranchDto>()
                    .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name));
                cfg.CreateMap<CreateBranchRequest, Branch>();
                cfg.CreateMap<UpdateBranchRequest, Branch>();
            });
            _mapper = config.CreateMapper();

            _service = new BranchService(_context, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedBranches_WithBusinessName()
        {
            // Arrange
            var business = new Business { Id = 1, Name = "ACME Corp" };
            _context.Businesses.Add(business);
            _context.Branches.Add(new Branch { Id = 5, Name = "North Branch", Business = business, BusinessId = business.Id });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            var dto = result.Items.First();
            dto.BusinessName.Should().Be("ACME Corp");
        }

        [Fact]
        public async Task GetByIdAsync_WhenBranchExists_ReturnsBranchDto()
        {
            // Arrange
            var business = new Business { Id = 2, Name = "XYZ Ltd" };
            _context.Businesses.Add(business);
            var branch = new Branch { Id = 10, Name = "South Branch", Business = business, BusinessId = business.Id };
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(10);

            // Assert
            result.Should().NotBeNull();
            result!.BusinessName.Should().Be("XYZ Ltd");
        }

        [Fact]
        public async Task GetByIdAsync_WhenBranchNotFound_ReturnsNull()
        {
            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }
    }
}
