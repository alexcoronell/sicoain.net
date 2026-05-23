using System.ComponentModel.DataAnnotations;
using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.EventCategories
{
    public record CreateEventCategoryRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public required string Name { get; init; }


        [Required, EnumDataType(typeof(AccidentSeverity))]
        public AccidentSeverity LevelOfSeverity { get; init; }

        public bool RequiresHospitalization { get; init; }
    }
}
