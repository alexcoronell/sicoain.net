using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Exceptions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Users;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IMapper _mapper;

        public UserService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }


        public async Task<PagedResponse<UserDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _userManager.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Id);

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync().ConfigureAwait(false);

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return new PagedResponse<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            if (user == null || user.IsDeleted) return null;
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request)
        {
            // Check for duplicate email before attempting creation
            var existingUser = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
            if (existingUser != null)
                throw new ConflictException($"Ya existe un usuario con el correo '{request.Email}'.");

            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (request.Roles != null && request.Roles.Any())
            {
                await AssignRolesToUser(user, request.Roles).ConfigureAwait(false);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            if (user == null || user.IsDeleted) return null;

            // Actualizar solo campos proporcionados
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                // Check for duplicate email before updating
                var existingUser = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
                if (existingUser != null)
                    throw new ConflictException($"Ya existe un usuario con el correo '{request.Email}'.");

                user.Email = request.Email;
                user.UserName = request.Email;
                user.NormalizedEmail = _userManager.NormalizeEmail(request.Email);
                user.NormalizedUserName = _userManager.NormalizeName(request.Email);
            }

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new InvalidOperationException("Error al actualizar el usuario: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            if (user == null || user.IsDeleted) return false;

            // Soft delete
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null || user.IsDeleted) return null;
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null) return false;
            return true;
        }

        public async Task<bool> AssignRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null || user.IsDeleted) return false;

            if (!await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
                return false;

            var result = await _userManager.AddToRoleAsync(user, roleName).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null || user.IsDeleted) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null || user.IsDeleted) return false;

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null || user.IsDeleted) return Enumerable.Empty<string>();

            return await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        }

        private async Task AssignRolesToUser(User user, List<string> roles)
        {
            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                    await _userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
            }
        }
    }
}
