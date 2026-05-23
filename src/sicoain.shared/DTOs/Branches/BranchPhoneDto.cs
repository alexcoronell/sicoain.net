namespace sicoain.shared.DTOs.Branch
{
    public record BranchPhoneDto : EntityPhoneDto
    {
        public int BranchId { get; init; }
    }
}
