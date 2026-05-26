using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Positions;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class PositionService : BaseService<Position, PositionDto, CreatePositionRequest, UpdatePositionRequest>, IPositionService
    {
        public PositionService(ApplicationDbContext context, IMapper _mapper) : base(context, _mapper)
        {
        }

        public override async Task<PagedResponse<PositionDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<Position>()
                .Include(e => e.Department)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

            return new PagedResponse<PositionDto>
            {
                Items = _mapper.Map<List<PositionDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public override async Task<PositionDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Position>()
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<PositionDto>(entity);
        }
    }
}
