using System.ComponentModel.DataAnnotations;
using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.AccidentTypes
{
    public record CreateAccidentTypeRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public required string Name { get; init; }

        [MaxLength(250)]
        public string? Description { get; init; }

        [Required, EnumDataType(typeof(AccidentSeverity))]
        public AccidentSeverity Severity { get; init; }
    }
}
