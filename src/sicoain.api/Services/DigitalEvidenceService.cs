using System.Security.Cryptography;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.DigitalEvidences;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class DigitalEvidenceService : IDigitalEvidenceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DigitalEvidenceService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResponse<DigitalEvidenceDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<DigitalEvidence>()
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

            return new PagedResponse<DigitalEvidenceDto>
            {
                Items = _mapper.Map<List<DigitalEvidenceDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<DigitalEvidenceDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<DigitalEvidence>().FindAsync(id).ConfigureAwait(false);
            return entity == null ? null : _mapper.Map<DigitalEvidenceDto>(entity);
        }

        public async Task<IEnumerable<DigitalEvidenceDto>> GetByAccidentIdAsync(int accidentId)
        {
            var query = _context.Set<DigitalEvidence>()
                .Where(a => a.AccidentId == accidentId)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.ToListAsync().ConfigureAwait(false);

            return _mapper.Map<IEnumerable<DigitalEvidenceDto>>(items);
        }

        /// <summary>
        /// Returns the base path used for file storage. Override in tests to avoid
        /// depending on the global current directory.
        /// </summary>
        protected virtual string GetCurrentBasePath() => Directory.GetCurrentDirectory();

        public async Task<DigitalEvidenceDto> UploadAsync(CreateDigitalEvidenceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Base64Content)) throw new ArgumentException("File content is required");
            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(request.Base64Content);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid Base64 string");
            }
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(fileBytes);
            var fileHash = Convert.ToHexString(hashBytes).ToLower();

            var uploadsFolder = Path.Combine(GetCurrentBasePath(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{request.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            await File.WriteAllBytesAsync(filePath, fileBytes).ConfigureAwait(false);

            var digitalEvidence = new DigitalEvidence
            {
                FileName = request.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                FileSize = fileBytes.Length,
                MimeType = request.MimeType,
                FileHash = fileHash,
                Description = request.Description,
                TakenAt = request.TakenAt,
                TakenByName = request.TakenByName,
                AccidentId = request.AccidentId,
                ChainOfCustody = request.ChainOfCustody,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Set<DigitalEvidence>().AddAsync(digitalEvidence).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<DigitalEvidenceDto>(digitalEvidence);
        }

        public async Task<DigitalEvidenceDto> UpdateMetadataAsync(int id, UpdateDigitalEvidenceRequest request)
        {
            var digitalEvidence = await _context.Set<DigitalEvidence>().FindAsync(id).ConfigureAwait(false);
            if (digitalEvidence == null) throw new KeyNotFoundException("Digital evidence not found");

            if (request.Description != null) digitalEvidence.Description = request.Description;

            _context.Set<DigitalEvidence>().Update(digitalEvidence);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<DigitalEvidenceDto>(digitalEvidence);
        }

        public async Task DeleteAsync(int id)
        {
            var digitalEvidence = await _context.Set<DigitalEvidence>().FindAsync(id).ConfigureAwait(false);

            if (digitalEvidence == null) throw new KeyNotFoundException("Digital evidence not found");

            var filePath = Path.Combine(GetCurrentBasePath(), "wwwroot", digitalEvidence.FilePath.TrimStart('/'));
            if (File.Exists(filePath)) File.Delete(filePath);

            _context.Set<DigitalEvidence>().Remove(digitalEvidence);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

}
