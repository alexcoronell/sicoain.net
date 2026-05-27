namespace sicoain.shared.DTOs
{
    public abstract record BaseDto
    {
        public int Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime? DeletedAt { get; init; }
        public int CreatedBy { get; init; }
        public int UpdatedBy { get; init; }
        public int? DeletedBy { get; init; }
    }
}
