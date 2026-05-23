using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Branch
{
    public record CreateBranchPhoneRequest : CreateEntityPhoneRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int BranchId { get; init; }
    }
}
