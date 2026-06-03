using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Branches;
using sicoain.shared.Entities;

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

            // Load navigation properties so the response DTO includes BusinessName
            await _context.Entry(entity).Reference(e => e.Business).LoadAsync().ConfigureAwait(false);

            return _mapper.Map<BranchDto>(entity);
        }

        public override async Task<PagedResponse<BranchDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<Branch>()
                .Include(e => e.Business)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

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
                .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<BranchDto>(entity);
        }
    }
}
