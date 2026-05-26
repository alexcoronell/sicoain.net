namespace sicoain.shared.DTOs.Branches
{
    public record BranchEmailDto : EntityEmailDto
    {
        public int BranchId { get; init; }
    }
}
