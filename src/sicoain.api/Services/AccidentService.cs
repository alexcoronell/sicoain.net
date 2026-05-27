using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Accident;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class AccidentService : BaseService<Accident, AccidentDto, CreateAccidentRequest, UpdateAccidentRequest>, IAccidentService
    {
        public AccidentService(ApplicationDbContext context, IMapper _mapper) : base(context, _mapper)
        {
        }

        public override async Task<PagedResponse<AccidentDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<Accident>()
                .Include(e => e.Employee)
                .Include(e => e.AccidentType)
                .Include(p => p.EventCategory)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

            return new PagedResponse<AccidentDto>
            {
                Items = _mapper.Map<List<AccidentDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public override async Task<AccidentDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Accident>()
                .Include(e => e.Employee)
                .Include(e => e.AccidentType)
                .Include(p => p.EventCategory)
                .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<AccidentDto>(entity);
        }
    }
}
