using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Employees;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class EmployeeService : BaseService<Employee, EmployeeDto, CreateEmployeeRequest, UpdateEmployeeRequest>, IEmployeeService
    {
        public EmployeeService(ApplicationDbContext context, IMapper _mapper) : base(context, _mapper)
        {
        }

        public override async Task<PagedResponse<EmployeeDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<Employee>()
                .Include(e => e.Business)
                .Include(e => e.Branch)
                .Include(e => e.Position)
                .ThenInclude(p => p!.Department)
                .AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

            return new PagedResponse<EmployeeDto>
            {
                Items = _mapper.Map<List<EmployeeDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public override async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Employee>()
                .Include(e => e.Business)
                .Include(e => e.Branch)
                .Include(e => e.Position)
                .ThenInclude(p => p!.Department)
                .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

            return entity == null ? null : _mapper.Map<EmployeeDto>(entity);
        }
    }
}
