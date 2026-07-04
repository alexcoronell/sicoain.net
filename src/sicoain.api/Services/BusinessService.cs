using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Business;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.api.Services
{
    public class BusinessService : BaseService<Business, BusinessDto, CreateBusinessRequest, UpdateBusinessRequest>, IBusinessService
    {
        public BusinessService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<PagedResponse<BusinessDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<Business>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return new PagedResponse<BusinessDto>
            {
                Items = _mapper.Map<List<BusinessDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public override async Task<BusinessDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Business>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .FirstOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<BusinessDto>(entity);
        }

        public override async Task<BusinessDto> CreateAsync(CreateBusinessRequest request)
        {
            var entity = _mapper.Map<Business>(request);
            _context.Set<Business>().Add(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Create emails from the typed list
            if (request.Emails is { Count: > 0 })
            {
                var emailEntities = request.Emails
                    .Where(e => !string.IsNullOrWhiteSpace(e.Email))
                    .Select(e => new BusinessEmail
                    {
                        BusinessId = entity.Id,
                        Email = e.Email.Trim(),
                        IsMain = e.IsMain,
                        Business = null!
                    }).ToList();
                IsMainHelper.EnsureSingleMain(emailEntities);
                _context.Set<BusinessEmail>().AddRange(emailEntities);
            }

            // Create phones from the typed list
            if (request.Phones is { Count: > 0 })
            {
                var phoneEntities = request.Phones
                    .Where(p => !string.IsNullOrWhiteSpace(p.Phone))
                    .Select(p => new BusinessPhone
                    {
                        BusinessId = entity.Id,
                        Phone = p.Phone.Trim(),
                        PhoneType = p.PhoneType,
                        IsMain = p.IsMain,
                        Business = null!
                    }).ToList();
                IsMainHelper.EnsureSingleMain(phoneEntities);
                _context.Set<BusinessPhone>().AddRange(phoneEntities);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Reload navigation properties for the response
            await _context.Entry(entity).Collection(e => e.Emails).LoadAsync().ConfigureAwait(false);
            await _context.Entry(entity).Collection(e => e.Phones).LoadAsync().ConfigureAwait(false);

            return _mapper.Map<BusinessDto>(entity);
        }

        public override async Task<BusinessDto?> UpdateAsync(int id, UpdateBusinessRequest request)
        {
            var entity = await _context.Set<Business>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .FirstOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);

            if (entity == null) return null;

            // Update business scalar fields
            _mapper.Map(request, entity);

            // ---- Sync emails ----
            if (request.Emails != null)
            {
                var requestedItems = request.Emails
                    .Where(e => !string.IsNullOrWhiteSpace(e.Email))
                    .ToList();

                var existingEmails = entity.Emails?.ToList() ?? [];

                // Remove items not in the request
                var requestedIds = requestedItems.Where(r => r.Id.HasValue).Select(r => r.Id!.Value).ToHashSet();
                var toRemove = existingEmails.Where(e => !requestedIds.Contains(e.Id)).ToList();
                if (toRemove.Count > 0)
                    _context.Set<BusinessEmail>().RemoveRange(toRemove);

                // Update existing items by Id
                foreach (var existing in existingEmails)
                {
                    var match = requestedItems.FirstOrDefault(r => r.Id == existing.Id);
                    if (match != null)
                    {
                        existing.Email = match.Email.Trim();
                        existing.IsMain = match.IsMain;
                    }
                }

                // Add new items (no Id)
                var newItems = requestedItems
                    .Where(r => !r.Id.HasValue)
                    .Select(r => new BusinessEmail
                    {
                        BusinessId = entity.Id,
                        Email = r.Email.Trim(),
                        IsMain = r.IsMain,
                        Business = null!
                    }).ToList();

                if (newItems.Count > 0)
                    _context.Set<BusinessEmail>().AddRange(newItems);

                // Normalize IsMain across all remaining emails
                var allEmails = existingEmails.Except(toRemove).Concat(newItems).ToList();
                IsMainHelper.EnsureSingleMain(allEmails);
            }

            // ---- Sync phones ----
            if (request.Phones != null)
            {
                var requestedItems = request.Phones
                    .Where(p => !string.IsNullOrWhiteSpace(p.Phone))
                    .ToList();

                var existingPhones = entity.Phones?.ToList() ?? [];

                // Remove items not in the request
                var requestedIds = requestedItems.Where(r => r.Id.HasValue).Select(r => r.Id!.Value).ToHashSet();
                var toRemove = existingPhones.Where(e => !requestedIds.Contains(e.Id)).ToList();
                if (toRemove.Count > 0)
                    _context.Set<BusinessPhone>().RemoveRange(toRemove);

                // Update existing items by Id
                foreach (var existing in existingPhones)
                {
                    var match = requestedItems.FirstOrDefault(r => r.Id == existing.Id);
                    if (match != null)
                    {
                        existing.Phone = match.Phone.Trim();
                        existing.PhoneType = match.PhoneType;
                        existing.IsMain = match.IsMain;
                    }
                }

                // Add new items (no Id)
                var newItems = requestedItems
                    .Where(r => !r.Id.HasValue)
                    .Select(r => new BusinessPhone
                    {
                        BusinessId = entity.Id,
                        Phone = r.Phone.Trim(),
                        PhoneType = r.PhoneType,
                        IsMain = r.IsMain,
                        Business = null!
                    }).ToList();

                if (newItems.Count > 0)
                    _context.Set<BusinessPhone>().AddRange(newItems);

                // Normalize IsMain across all remaining phones
                var allPhones = existingPhones.Except(toRemove).Concat(newItems).ToList();
                IsMainHelper.EnsureSingleMain(allPhones);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<BusinessDto>(entity);
        }
    }
}
