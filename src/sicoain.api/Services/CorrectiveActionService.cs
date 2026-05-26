using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.CorrectiveActions;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class CorrectiveActionService : BaseService<CorrectiveAction, CorrectiveActionDto, CreateCorrectiveActionRequest, UpdateCorrectiveActionRequest>, ICorrectiveActionService
    {
        public CorrectiveActionService(ApplicationDbContext context, IMapper _mapper) : base(context, _mapper)
        {
        }

        public override async Task<PagedResponse<CorrectiveActionDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<CorrectiveAction>()
                .Include(e => e.Accident)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

            return new PagedResponse<CorrectiveActionDto>
            {
                Items = _mapper.Map<List<CorrectiveActionDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public override async Task<CorrectiveActionDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<CorrectiveAction>()
                .Include(e => e.Accident)
                .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<CorrectiveActionDto>(entity);
        }
    }
}
