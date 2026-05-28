using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.Attachments;
using sicoain.shared.Entities;
using sicoain.shared.Enums;
using System.Security.Cryptography;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class AttachmentServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _testUploadFolder;
        private readonly AttachmentService _service;

        public AttachmentServiceTests()
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
                cfg.CreateMap<Attachment, AttachmentDto>();
                cfg.CreateMap<CreateAttachmentRequest, Attachment>();
                cfg.CreateMap<UpdateAttachmentRequest, Attachment>();
            });
            _mapper = config.CreateMapper();

            _service = new TestableAttachmentService(_context, _mapper, _testUploadFolder);
        }

        public void Dispose()
        {
            // Clean up the temporary directory
            if (Directory.Exists(_testUploadFolder))
            {
                Directory.Delete(_testUploadFolder, true);
            }
            _context.Dispose();
        }

        /// <summary>
        /// Testable subclass that overrides GetCurrentBasePath to use a temp directory
        /// instead of the process-wide current directory, avoiding parallel test interference.
        /// </summary>
        private sealed class TestableAttachmentService : AttachmentService
        {
            private readonly string _basePath;

            public TestableAttachmentService(ApplicationDbContext context, IMapper mapper, string basePath)
                : base(context, mapper)
            {
                _basePath = basePath;
            }

            protected override string GetCurrentBasePath() => _basePath;
        }

        private CreateAttachmentRequest CreateValidRequest() => new()
        {
            FileName = "test.jpg",
            MimeType = "image/jpeg",
            Description = "Test image",
            EntityType = AttachmentEntityType.Accident,
            EntityId = 1,
            Base64Content = Convert.ToBase64String(new byte[] { 0xFF, 0xD8 }) // dummy JPEG header
        };

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedAttachments()
        {
            // Arrange
            _context.Attachments.Add(new Attachment { Id = 1, FileName = "a.txt", FilePath = "/a.txt", FileSize = 100, MimeType = "text/plain", FileHash = "hash1", Description = "File A", EntityType = AttachmentEntityType.Accident });
            _context.Attachments.Add(new Attachment { Id = 2, FileName = "b.txt", FilePath = "/b.txt", FileSize = 200, MimeType = "text/plain", FileHash = "hash2", Description = "File B", EntityType = AttachmentEntityType.Accident });
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
            var entity = new Attachment { Id = 5, FileName = "doc.pdf", FilePath = "/doc.pdf", FileSize = 500, MimeType = "application/pdf", FileHash = "hash5", Description = "Doc", EntityType = AttachmentEntityType.Employee };
            _context.Attachments.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.FileName.Should().Be("doc.pdf");
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
        public async Task GetByEntityIdAsync_ShouldReturnAttachmentsForEntity()
        {
            // Arrange
            _context.Attachments.Add(new Attachment { Id = 3, FileName = "f3.txt", FilePath = "/f3.txt", FileSize = 300, MimeType = "text/plain", FileHash = "hash3", Description = "File 3", EntityType = AttachmentEntityType.Accident, EntityId = 10 });
            _context.Attachments.Add(new Attachment { Id = 4, FileName = "f4.txt", FilePath = "/f4.txt", FileSize = 400, MimeType = "text/plain", FileHash = "hash4", Description = "File 4", EntityType = AttachmentEntityType.Accident, EntityId = 20 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByEntityIdAsync(AttachmentEntityType.Accident, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(3);
        }

        [Fact]
        public async Task UploadAsync_WithValidRequest_SavesFileAndReturnsDto()
        {
            // Arrange
            var request = CreateValidRequest();

            // Act
            var result = await _service.UploadAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.FileName.Should().Be(request.FileName);
            result.FileSize.Should().BeGreaterThan(0);
            var saved = await _context.Attachments.FindAsync(result.Id);
            saved.Should().NotBeNull();
            // Check file was saved on disk
            var fullPath = Path.Combine(_testUploadFolder, "wwwroot", saved!.FilePath.TrimStart('/'));
            File.Exists(fullPath).Should().BeTrue();
        }

        [Fact]
        public async Task UploadAsync_WithEmptyBase64_ThrowsArgumentException()
        {
            // Arrange
            var request = CreateValidRequest() with { Base64Content = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UploadAsync(request));
        }

        [Fact]
        public async Task UpdateMetadataAsync_WhenExists_UpdatesDescription()
        {
            // Arrange
            var entity = new Attachment { Id = 7, FileName = "old.txt", FilePath = "/old.txt", FileSize = 700, MimeType = "text/plain", FileHash = "hash7", Description = "Old description", EntityType = AttachmentEntityType.Accident };
            _context.Attachments.Add(entity);
            await _context.SaveChangesAsync();
            var updateRequest = new UpdateAttachmentRequest { Description = "New description" };

            // Act
            var result = await _service.UpdateMetadataAsync(7, updateRequest);

            // Assert
            result.Description.Should().Be("New description");
            var updated = await _context.Attachments.FindAsync(7);
            updated!.Description.Should().Be("New description");
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
            var deleted = await _context.Attachments.FindAsync(uploaded.Id);
            deleted.Should().BeNull();
            File.Exists(filePath).Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WhenNotExists_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999));
        }
    }
}
