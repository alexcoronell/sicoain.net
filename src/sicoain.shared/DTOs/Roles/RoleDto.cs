namespace sicoain.shared.DTOs.Roles
{
    public record RoleDto : BaseDto
    {
        public int IdentityRoleId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsActive { get; init; }

    }
}
