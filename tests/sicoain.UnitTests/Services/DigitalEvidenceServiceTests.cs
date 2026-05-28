using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.DigitalEvidences;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class DigitalEvidenceServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _testUploadFolder;
        private readonly DigitalEvidenceService _service;

        public DigitalEvidenceServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Create a temporary folder for uploads (no SetCurrentDirectory needed)
            _testUploadFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testUploadFolder);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DigitalEvidence, DigitalEvidenceDto>();
                cfg.CreateMap<CreateDigitalEvidenceRequest, DigitalEvidence>();
                cfg.CreateMap<UpdateDigitalEvidenceRequest, DigitalEvidence>();
            });
            _mapper = config.CreateMapper();

            _service = new TestableDigitalEvidenceService(_context, _mapper, _testUploadFolder);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testUploadFolder))
                Directory.Delete(_testUploadFolder, true);
            _context.Dispose();
        }

        /// <summary>
        /// Testable subclass that overrides GetCurrentBasePath to use a temp directory
        /// instead of the process-wide current directory, avoiding parallel test interference.
        /// </summary>
        private sealed class TestableDigitalEvidenceService : DigitalEvidenceService
        {
            private readonly string _basePath;

            public TestableDigitalEvidenceService(ApplicationDbContext context, IMapper mapper, string basePath)
                : base(context, mapper)
            {
                _basePath = basePath;
            }

            protected override string GetCurrentBasePath() => _basePath;
        }

        private CreateDigitalEvidenceRequest CreateValidRequest() => new()
        {
            FileName = "video.mp4",
            MimeType = "video/mp4",
            Description = "Accident video",
            TakenAt = DateTime.UtcNow,
            TakenByName = "Officer",
            AccidentId = 5,
            Base64Content = Convert.ToBase64String(new byte[] { 0x00, 0x01, 0x02 })
        };

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedEvidences()
        {
            // Arrange
            _context.DigitalEvidences.Add(new DigitalEvidence { Id = 1, FileName = "a.mp4", FilePath = "/a.mp4", FileSize = 1000, MimeType = "video/mp4", FileHash = "hash1", Description = "Video A", TakenAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccidentId = 0 });
            _context.DigitalEvidences.Add(new DigitalEvidence { Id = 2, FileName = "b.mp4", FilePath = "/b.mp4", FileSize = 2000, MimeType = "video/mp4", FileHash = "hash2", Description = "Video B", TakenAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccidentId = 0 });
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
            var entity = new DigitalEvidence { Id = 5, FileName = "doc.pdf", FilePath = "/doc.pdf", FileSize = 500, MimeType = "application/pdf", FileHash = "hash5", Description = "Doc", TakenAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccidentId = 0 };
            _context.DigitalEvidences.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.FileName.Should().Be("doc.pdf");
        }

        [Fact]
        public async Task GetByAccidentIdAsync_ReturnsEvidencesForAccident()
        {
            // Arrange
            _context.DigitalEvidences.Add(new DigitalEvidence { Id = 10, FileName = "ev10.mp4", FilePath = "/ev10.mp4", FileSize = 1000, MimeType = "video/mp4", FileHash = "hash10", Description = "Ev 10", TakenAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccidentId = 100 });
            _context.DigitalEvidences.Add(new DigitalEvidence { Id = 11, FileName = "ev11.mp4", FilePath = "/ev11.mp4", FileSize = 1100, MimeType = "video/mp4", FileHash = "hash11", Description = "Ev 11", TakenAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccidentId = 200 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByAccidentIdAsync(100);

            // Assert
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(10);
        }

        [Fact]
        public async Task UploadAsync_WithValidRequest_SavesEvidence()
        {
            // Arrange
            var request = CreateValidRequest();

            // Act
            var result = await _service.UploadAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.FileName.Should().Be(request.FileName);
            var saved = await _context.DigitalEvidences.FindAsync(result.Id);
            saved.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateMetadataAsync_WhenExists_UpdatesFields()
        {
            // Arrange
            var entity = new DigitalEvidence { Id = 8, FileName = "old.mp4", FilePath = "/old.mp4", FileSize = 800, MimeType = "video/mp4", FileHash = "hash8", Description = "Old", TakenAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccidentId = 0 };
            _context.DigitalEvidences.Add(entity);
            await _context.SaveChangesAsync();
            var updateRequest = new UpdateDigitalEvidenceRequest { Description = "New description" };

            // Act
            var result = await _service.UpdateMetadataAsync(8, updateRequest);

            // Assert
            result.Description.Should().Be("New description");
        }

        [Fact]
        public async Task DeleteAsync_WhenExists_RemovesFileAndRecord()
        {
            // Arrange
            var request = CreateValidRequest();
            var uploaded = await _service.UploadAsync(request);
            var filePath = Path.Combine(_testUploadFolder, "wwwroot", uploaded.FilePath.TrimStart('/'));
            File.Exists(filePath).Should().BeTrue();

            // Act
            await _service.DeleteAsync(uploaded.Id);

            // Assert
            var deleted = await _context.DigitalEvidences.FindAsync(uploaded.Id);
            deleted.Should().BeNull();
            File.Exists(filePath).Should().BeFalse();
        }
    }
}
