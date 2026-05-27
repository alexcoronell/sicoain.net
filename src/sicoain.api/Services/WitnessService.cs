using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Witnesses;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class WitnessService : BaseService<Witness, WitnessDto, CreateWitnessRequest, UpdateWitnessRequest>, IWitnessService
    {
        public WitnessService(ApplicationDbContext context, IMapper _mapper) : base(context, _mapper)
        {
        }

        public override async Task<PagedResponse<WitnessDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<Witness>()
                .Include(e => e.Accident)
                .Include(e => e.Employee)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

            return new PagedResponse<WitnessDto>
            {
                Items = _mapper.Map<List<WitnessDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public override async Task<WitnessDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Witness>()
                .Include(e => e.Accident)
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<WitnessDto>(entity);
        }
    }
}
