using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Attachments;
using sicoain.shared.Enums;
using sicoain.shared.Entities;
using System.Security.Cryptography;
using sicoain.api.Data;

namespace sicoain.api.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AttachmentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResponse<AttachmentDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<Attachment>()
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

            return new PagedResponse<AttachmentDto>
            {
                Items = _mapper.Map<List<AttachmentDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<AttachmentDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Attachment>().FindAsync(id).ConfigureAwait(false);
            return entity == null ? null : _mapper.Map<AttachmentDto>(entity);
        }

        public async Task<IEnumerable<AttachmentDto>> GetByEntityIdAsync(AttachmentEntityType entityType, int id)
        {
            var attachments = await _context.Set<Attachment>()
                .Where(a => a.EntityType == entityType && a.EntityId == id)
                .ToListAsync().ConfigureAwait(false);

            return _mapper.Map<IEnumerable<AttachmentDto>>(attachments);
        }

        /// <summary>
        /// Returns the base path used for file storage. Override in tests to avoid
        /// depending on the global current directory.
        /// </summary>
        protected virtual string GetCurrentBasePath() => Directory.GetCurrentDirectory();

        public async Task<AttachmentDto> UploadAsync(CreateAttachmentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Base64Content)) throw new ArgumentException("File content is required");

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(request.Base64Content);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid Base64 content");
            }

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(fileBytes);
            var fileHash = Convert.ToHexString(hashBytes).ToLower();

            var uploadsFolder = Path.Combine(GetCurrentBasePath(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{request.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            await File.WriteAllBytesAsync(filePath, fileBytes).ConfigureAwait(false);

            var attachment = new Attachment
            {
                FileName = request.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                FileSize = fileBytes.Length,
                MimeType = request.MimeType,
                FileHash = fileHash,
                Description = request.Description,
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Set<Attachment>().AddAsync(attachment).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<AttachmentDto>(attachment);
        }

        public async Task<AttachmentDto> UpdateMetadataAsync(int id, UpdateAttachmentRequest request)
        {
            var attachment = await _context.Set<Attachment>().FindAsync(id).ConfigureAwait(false);
            if (attachment == null) throw new KeyNotFoundException("Attachment not found");

            if (request.Description != null) attachment.Description = request.Description;

            _context.Set<Attachment>().Update(attachment);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return _mapper.Map<AttachmentDto>(attachment);
        }


        public async Task DeleteAsync(int id)
        {
            var attachment = await _context.Set<Attachment>().FindAsync(id).ConfigureAwait(false);

            if (attachment == null) throw new KeyNotFoundException("Attachment not found");

            var filePath = Path.Combine(GetCurrentBasePath(), "wwwroot", attachment.FilePath.TrimStart('/'));
            if (File.Exists(filePath)) File.Delete(filePath);

            _context.Set<Attachment>().Remove(attachment);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
