using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs;

namespace sicoain.api.Services
{
    /// <summary>
    /// Generic base service implementing CRUD operations with pagination
    /// </summary>
    /// <typeparam name="TEntity">Entity type (e.g., Employee, Accident)</typeparam>
    /// <typeparam name="TDto">DTO type for responses (e.g., EmployeeDto)</typeparam>
    /// <typeparam name="TCreateRequest">DTO type for create requests</typeparam>
    /// <typeparam name="TUpdateRequest">DTO type for update requests</typeparam>
    public abstract class BaseService<TEntity, TDto, TCreateRequest, TUpdateRequest>
        : IBaseService<TDto, TCreateRequest, TUpdateRequest>
        where TEntity : class
        where TDto : class
        where TCreateRequest : class
        where TUpdateRequest : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IMapper _mapper;

        protected BaseService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public virtual async Task<PagedResponse<TDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return new PagedResponse<TDto>
            {
                Items = _mapper.Map<List<TDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <inheritdoc />
        public virtual async Task<TDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
            return entity == null ? null : _mapper.Map<TDto>(entity);
        }

        /// <inheritdoc />
        public virtual async Task<TDto> CreateAsync(TCreateRequest request)
        {
            var entity = _mapper.Map<TEntity>(request);
            await _context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<TDto>(entity);
        }

        /// <inheritdoc />
        public virtual async Task<TDto?> UpdateAsync(int id, TUpdateRequest request)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _mapper.Map<TDto>(entity);
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
            if (entity == null) return false;

            // Soft delete - si la entidad tiene propiedad IsDeleted
            var property = entity.GetType().GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, true);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }

            // Hard delete (solo si no tiene soft delete)
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Checks if an entity exists by its ID
        /// </summary>
        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<TEntity>().AnyAsync(e => EF.Property<int>(e, "Id") == id).ConfigureAwait(false);
        }
    }
}
