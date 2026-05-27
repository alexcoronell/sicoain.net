using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.EventCategories
{
    public record EventCategoryDto : BaseDto
    {
        public string? Name { get; init; }
        public AccidentSeverity LevelOfSeverity { get; init; }

        public bool RequiresHospitalization { get; init; }
    }
}
