namespace sicoain.shared.DTOs.Branch
{
    public record UpdateBranchEmailRequest : UpdateEntityEmailRequest
    {
        public int BranchId { get; init; }
    }
}
