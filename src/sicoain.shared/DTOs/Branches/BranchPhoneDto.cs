namespace sicoain.shared.DTOs.Branches
{
    public record BranchPhoneDto : EntityPhoneDto
    {
        public int BranchId { get; init; }
    }
}
