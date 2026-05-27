namespace sicoain.shared.DTOs.Business
{
    public record BusinessPhoneDto : EntityPhoneDto
    {
        public int BusinessId { get; init; }
    }
}
