using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.AccidentTypes
{
    public record AccidentTypeDto : BaseDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public AccidentSeverity Severity { get; init; }
    }
}
