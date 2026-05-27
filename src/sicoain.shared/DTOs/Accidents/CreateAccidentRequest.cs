using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Accident
{
    public record CreateAccidentRequest
    {
        [Required, DataType(DataType.Date)]
        public DateTime EventDate { get; init; }

        [Required, MinLength(10), MaxLength(500)]
        public required string Description { get; init; }

        [Required, Range(1, int.MaxValue)]
        public int EmployeeId { get; init; }

        [Required, Range(1, int.MaxValue)]
        public int AccidentTypeId { get; init; }

        [Required, Range(1, int.MaxValue)]
        public int EventCategoryId { get; init; }
    }
}
