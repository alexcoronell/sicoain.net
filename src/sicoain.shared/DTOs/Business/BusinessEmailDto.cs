namespace sicoain.shared.DTOs.Business
{
    public record BusinessEmailDto : EntityEmailDto
    {
        public int BusinessId { get; init; }
    }
}
