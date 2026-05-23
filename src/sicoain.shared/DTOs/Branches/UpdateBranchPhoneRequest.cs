namespace sicoain.shared.DTOs.Branch
{
    public record UpdateBranchPhoneRequest : UpdateEntityPhoneRequest
    {
        public int BranchId { get; init; }
    }
}
