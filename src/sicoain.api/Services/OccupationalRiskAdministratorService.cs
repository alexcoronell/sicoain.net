using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.api.Services
{
    public class OccupationalRiskAdministratorService : BaseService<OccupationalRiskAdministrator, OccupationalRiskAdministratorDto, CreateOccupationalRiskAdministratorRequest, UpdateOccupationalRiskAdministratorRequest>, IOccupationalRiskAdministratorService
    {
        public OccupationalRiskAdministratorService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<PagedResponse<OccupationalRiskAdministratorDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<OccupationalRiskAdministrator>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return new PagedResponse<OccupationalRiskAdministratorDto>
            {
                Items = _mapper.Map<List<OccupationalRiskAdministratorDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public override async Task<OccupationalRiskAdministratorDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<OccupationalRiskAdministrator>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .FirstOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<OccupationalRiskAdministratorDto>(entity);
        }

        public override async Task<OccupationalRiskAdministratorDto> CreateAsync(CreateOccupationalRiskAdministratorRequest request)
        {
            var entity = _mapper.Map<OccupationalRiskAdministrator>(request);
            _context.Set<OccupationalRiskAdministrator>().Add(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Create emails from the string list
            if (request.Emails is { Count: > 0 })
            {
                var emails = request.Emails
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(e => new OccupationalRiskAdministratorEmail
                    {
                        OccupationalRiskAdministratorId = entity.Id,
                        Email = e.Trim(),
                        OccupationalRiskAdministrator = null!
                    });
                _context.Set<OccupationalRiskAdministratorEmail>().AddRange(emails);
            }

            // Create phones from the string list (default to Mobile type)
            if (request.Phones is { Count: > 0 })
            {
                var phones = request.Phones
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => new OccupationalRiskAdministratorPhone
                    {
                        OccupationalRiskAdministratorId = entity.Id,
                        Phone = p.Trim(),
                        PhoneType = PhoneType.Mobile,
                        OccupationalRiskAdministrator = null!
                    });
                _context.Set<OccupationalRiskAdministratorPhone>().AddRange(phones);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Reload navigation properties for the response
            await _context.Entry(entity).Collection(e => e.Emails).LoadAsync().ConfigureAwait(false);
            await _context.Entry(entity).Collection(e => e.Phones).LoadAsync().ConfigureAwait(false);

            return _mapper.Map<OccupationalRiskAdministratorDto>(entity);
        }

        public override async Task<OccupationalRiskAdministratorDto?> UpdateAsync(int id, UpdateOccupationalRiskAdministratorRequest request)
        {
            var entity = await _context.Set<OccupationalRiskAdministrator>()
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
                    _context.Set<OccupationalRiskAdministratorEmail>().RemoveRange(emailsToRemove);

                // Add new emails
                var emailsToAdd = requestedEmails
                    .Where(e => !existingEmails.Contains(e))
                    .Select(e => new OccupationalRiskAdministratorEmail
                    {
                        OccupationalRiskAdministratorId = entity.Id,
                        Email = e,
                        OccupationalRiskAdministrator = null!
                    });
                if (emailsToAdd.Any())
                    _context.Set<OccupationalRiskAdministratorEmail>().AddRange(emailsToAdd);
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
                    _context.Set<OccupationalRiskAdministratorPhone>().RemoveRange(phonesToRemove);

                // Add new phones
                var phonesToAdd = requestedPhones
                    .Where(p => !existingPhones.Contains(p))
                    .Select(p => new OccupationalRiskAdministratorPhone
                    {
                        OccupationalRiskAdministratorId = entity.Id,
                        Phone = p,
                        PhoneType = PhoneType.Mobile,
                        OccupationalRiskAdministrator = null!
                    });
                if (phonesToAdd.Any())
                    _context.Set<OccupationalRiskAdministratorPhone>().AddRange(phonesToAdd);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<OccupationalRiskAdministratorDto>(entity);
        }
    }
}
