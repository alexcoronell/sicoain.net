namespace sicoain.shared.DTOs.DigitalEvidences
{
    public record DigitalEvidenceDto : BaseDto
    {
        public string FileName { get; init; } = string.Empty;
        public string FilePath { get; init; } = string.Empty;
        public long FileSize { get; init; }
        public string MimeType { get; init; } = string.Empty;
        public string FileHash { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime TakenAt { get; init; }
        public string? TakenByName { get; init; }
        public string ChainOfCustody { get; init; } = string.Empty;
        public int? AccidentId { get; init; }
    }
}
