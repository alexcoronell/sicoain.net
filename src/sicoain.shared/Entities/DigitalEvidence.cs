using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class DigitalEvidence : BaseEntity
    {
        [Required]
        public required string FileName { get; set; }

        [Required]
        public required string FilePath { get; set; }

        [Required]
        public required long FileSize { get; set; }

        [Required]
        public required string MimeType { get; set; }

        [Required]
        public required string FileHash { get; set; }

        [Required]
        public required string Description { get; set; } = string.Empty;

        [Required]
        public required DateTime TakenAt { get; set; }

        public string? TakenByName { get; set; }

        public string ChainOfCustody { get; set; } = string.Empty;

        [Required]
        public int? AccidentId { get; set; }

        public Accident? Accident { get; set; }
    }
}
