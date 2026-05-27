using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Business
{
    public record CreateBusinessPhoneRequest : CreateEntityPhoneRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int BusinessId { get; init; }
    }
}
