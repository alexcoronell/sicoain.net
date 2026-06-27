using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Branches;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.api.Services
{
    public class BranchService : BaseService<Branch, BranchDto, CreateBranchRequest, UpdateBranchRequest>, IBranchService
    {
        public BranchService(ApplicationDbContext context, IMapper _mapper) : base(context, _mapper)
        {
        }

        public override async Task<BranchDto> CreateAsync(CreateBranchRequest request)
        {
            var entity = _mapper.Map<Branch>(request);
            _context.Set<Branch>().Add(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Create emails from the string list
            if (request.Emails is { Count: > 0 })
            {
                var emails = request.Emails
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(e => new BranchEmail
                    {
                        BranchId = entity.Id,
                        Email = e.Trim(),
                        Branch = null!
                    });
                _context.Set<BranchEmail>().AddRange(emails);
            }

            // Create phones from the string list
            if (request.Phones is { Count: > 0 })
            {
                var phones = request.Phones
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => new BranchPhone
                    {
                        BranchId = entity.Id,
                        Phone = p.Trim(),
                        PhoneType = PhoneType.Mobile,
                        Branch = null!
                    });
                _context.Set<BranchPhone>().AddRange(phones);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);

            await _context.Entry(entity).Collection(e => e.Emails).LoadAsync().ConfigureAwait(false);
            await _context.Entry(entity).Collection(p => p.Phones).LoadAsync().ConfigureAwait(false);

            return _mapper.Map<BranchDto>(entity);


        }

        public override async Task<PagedResponse<BranchDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<Branch>()
                .Include(e => e.Business)
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return new PagedResponse<BranchDto>
            {
                Items = _mapper.Map<List<BranchDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public override async Task<BranchDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Branch>()
                .Include(e => e.Business)
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .FirstOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<BranchDto>(entity);
        }

        public override async Task<BranchDto?> UpdateAsync(int id, UpdateBranchRequest request)
        {
            var entity = await _context.Set<Branch>()
                .Include(e => e.Emails)
                .Include(e => e.Phones)
                .FirstOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);

            if (entity == null) return null;

            // Update branch scalar fields
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
                    _context.Set<BranchEmail>().RemoveRange(emailsToRemove);

                // Add new emails
                var emailsToAdd = requestedEmails
                    .Where(e => !existingEmails.Contains(e))
                    .Select(e => new BranchEmail
                    {
                        BranchId = entity.Id,
                        Email = e,
                        Branch = null!
                    });
                if (emailsToAdd.Any())
                    _context.Set<BranchEmail>().AddRange(emailsToAdd);
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
                    _context.Set<BranchPhone>().RemoveRange(phonesToRemove);

                // Add new phones
                var phonesToAdd = requestedPhones
                    .Where(p => !existingPhones.Contains(p))
                    .Select(p => new BranchPhone
                    {
                        BranchId = entity.Id,
                        Phone = p,
                        PhoneType = PhoneType.Mobile,
                        Branch = null!
                    });
                if (phonesToAdd.Any())
                    _context.Set<BranchPhone>().AddRange(phonesToAdd);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<BranchDto>(entity);
        }
    }
}
