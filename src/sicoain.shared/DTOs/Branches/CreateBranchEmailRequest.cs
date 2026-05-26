using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Branches
{
    public record CreateBranchEmailRequest : CreateEntityEmailRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int BranchId { get; init; }
    }
}
