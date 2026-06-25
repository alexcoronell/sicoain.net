using sicoain.shared.DTOs;

namespace sicoain.shared.DTOs.Permissions;

public record PermissionDto : BaseDto
{
    public string Name { get; init; } = string.Empty;
    public string Module { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string? Description { get; init; }
}
