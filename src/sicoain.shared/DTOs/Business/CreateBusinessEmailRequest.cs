using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Business
{
    public record CreateBusinessEmailRequest : CreateEntityEmailRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int BusinessId { get; init; }
    }
}
