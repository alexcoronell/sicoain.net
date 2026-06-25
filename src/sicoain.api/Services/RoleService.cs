using AutoMapper;
using Microsoft.AspNetCore.Identity;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.api.Exceptions;
using sicoain.shared.DTOs.Roles;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class RoleService : BaseService<Roles, RoleDto, CreateRoleRequest, UpdateRoleRequest>, IRoleService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public RoleService(ApplicationDbContext context, IMapper mapper, RoleManager<IdentityRole<int>> roleManager)
            : base(context, mapper)
        {
            _roleManager = roleManager;
        }

        public override async Task<RoleDto> CreateAsync(CreateRoleRequest request)
        {
            // 1. Create the Identity role FIRST so we get a valid IdentityRoleId
            var identityRole = new IdentityRole<int> { Name = request.Name };
            var identityResult = await _roleManager.CreateAsync(identityRole).ConfigureAwait(false);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join("; ", identityResult.Errors.Select(e => e.Description));
                throw new ConflictException($"No se pudo crear el rol en Identity: {errors}");
            }

            // 2. Now create the custom Roles entity linked to the Identity role
            var entity = _mapper.Map<Roles>(request);
            entity.IdentityRoleId = identityRole.Id;
            entity.NormalizedName = identityRole.NormalizedName;
            entity.CreatedAt = DateTime.UtcNow;

            await _context.Set<Roles>().AddAsync(entity).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return _mapper.Map<RoleDto>(entity);
        }
    }
}
