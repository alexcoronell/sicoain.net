namespace sicoain.shared.DTOs.DigitalEvidences
{
    public record UpdateDigitalEvidenceRequest
    {
        public string? FileName { get; init; }
        public string? Description { get; init; }
        public DateTime? TakenAt { get; init; }
        public string? TakenByName { get; init; }
        public string? ChainOfCustody { get; init; }
    }
}
