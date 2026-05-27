using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.DigitalEvidences
{
    public record CreateDigitalEvidenceRequest
    {
        [Required]
        public required string FileName { get; init; }

        [Required]
        public required string MimeType { get; init; }

        [Required]
        public required string Description { get; init; } = string.Empty;

        [Required, DataType(DataType.DateTime)]
        public required DateTime TakenAt { get; init; }

        public string? TakenByName { get; init; }

        public string ChainOfCustody { get; init; } = string.Empty;

        public int? AccidentId { get; init; }   // Opcional, según lógica de negocio

        [Required]
        public required string Base64Content { get; init; }
    }
}
