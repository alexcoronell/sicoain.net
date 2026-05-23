namespace sicoain.shared.DTOs.Branch
{
    public record BranchEmailDto : EntityEmailDto
    {
        public int BranchId { get; init; }
    }
}
