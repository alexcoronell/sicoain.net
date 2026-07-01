using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.HealthPromotionEntities;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.api.Services
{
    public class HealthPromotionEntityService : BaseService<HealthPromotionEntity, HealthPromotionEntityDto, CreateHealthPromotionEntityRequest, UpdateHealthPromotionEntityRequest>, IHealthPromotionEntityService
    {
        public HealthPromotionEntityService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<PagedResponse<HealthPromotionEntityDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<HealthPromotionEntity>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return new PagedResponse<HealthPromotionEntityDto>
            {
                Items = _mapper.Map<List<HealthPromotionEntityDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public override async Task<HealthPromotionEntityDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<HealthPromotionEntity>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .FirstOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<HealthPromotionEntityDto>(entity);
        }

        public override async Task<HealthPromotionEntityDto> CreateAsync(CreateHealthPromotionEntityRequest request)
        {
            var entity = _mapper.Map<HealthPromotionEntity>(request);
            _context.Set<HealthPromotionEntity>().Add(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Create emails from the string list
            if (request.Emails is { Count: > 0 })
            {
                var emails = request.Emails
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(e => new HealthPromotionEntityEmail
                    {
                        HealthPromotionEntityId = entity.Id,
                        Email = e.Trim(),
                        HealthPromotionEntity = null!
                    });
                _context.Set<HealthPromotionEntityEmail>().AddRange(emails);
            }

            // Create phones from the string list (default to Mobile type)
            if (request.Phones is { Count: > 0 })
            {
                var phones = request.Phones
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => new HealthPromotionEntityPhone
                    {
                        HealthPromotionEntityId = entity.Id,
                        Phone = p.Trim(),
                        PhoneType = PhoneType.Mobile,
                        HealthPromotionEntity = null!
                    });
                _context.Set<HealthPromotionEntityPhone>().AddRange(phones);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Reload navigation properties for the response
            await _context.Entry(entity).Collection(e => e.Emails).LoadAsync().ConfigureAwait(false);
            await _context.Entry(entity).Collection(e => e.Phones).LoadAsync().ConfigureAwait(false);

            return _mapper.Map<HealthPromotionEntityDto>(entity);
        }

        public override async Task<HealthPromotionEntityDto?> UpdateAsync(int id, UpdateHealthPromotionEntityRequest request)
        {
            var entity = await _context.Set<HealthPromotionEntity>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .FirstOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);

            if (entity == null) return null;

            // Update entity scalar fields
            _mapper.Map(request, entity);

            // ---- Sync emails ----
            if (request.Emails != null)
            {
                var requestedEmails = request.Emails
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(e => e.Trim())
                    .ToHashSet(StringComparer.Ordinal);

                var existingEmails = entity.Emails?.Select(e => e.Email).ToHashSet(StringComparer.Ordinal) ?? [];

                // Remove emails that are no longer in the list
                var emailsToRemove = entity.Emails?
                    .Where(e => !requestedEmails.Contains(e.Email))
                    .ToList();
                if (emailsToRemove?.Count > 0)
                    _context.Set<HealthPromotionEntityEmail>().RemoveRange(emailsToRemove);

                // Add new emails
                var emailsToAdd = requestedEmails
                    .Where(e => !existingEmails.Contains(e))
                    .Select(e => new HealthPromotionEntityEmail
                    {
                        HealthPromotionEntityId = entity.Id,
                        Email = e,
                        HealthPromotionEntity = null!
                    });
                if (emailsToAdd.Any())
                    _context.Set<HealthPromotionEntityEmail>().AddRange(emailsToAdd);
            }

            // ---- Sync phones ----
            if (request.Phones != null)
            {
                var requestedPhones = request.Phones
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim())
                    .ToHashSet(StringComparer.Ordinal);

                var existingPhones = entity.Phones?.Select(p => p.Phone).ToHashSet(StringComparer.Ordinal) ?? [];

                // Remove phones that are no longer in the list
                var phonesToRemove = entity.Phones?
                    .Where(p => !requestedPhones.Contains(p.Phone))
                    .ToList();
                if (phonesToRemove?.Count > 0)
                    _context.Set<HealthPromotionEntityPhone>().RemoveRange(phonesToRemove);

                // Add new phones
                var phonesToAdd = requestedPhones
                    .Where(p => !existingPhones.Contains(p))
                    .Select(p => new HealthPromotionEntityPhone
                    {
                        HealthPromotionEntityId = entity.Id,
                        Phone = p,
                        PhoneType = PhoneType.Mobile,
                        HealthPromotionEntity = null!
                    });
                if (phonesToAdd.Any())
                    _context.Set<HealthPromotionEntityPhone>().AddRange(phonesToAdd);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<HealthPromotionEntityDto>(entity);
        }
    }
}
