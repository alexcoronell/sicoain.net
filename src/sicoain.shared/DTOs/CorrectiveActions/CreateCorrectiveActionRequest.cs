using System.ComponentModel.DataAnnotations;
using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.CorrectiveActions
{
    public record CreateCorrectiveActionRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public required string Title { get; init; }
        [Required, MinLength(3), MaxLength(500)]
        public required string Description { get; init; }

        [Required, DataType(DataType.Date)]
        public required DateTime DueDate { get; init; }
        public required StatusAction? Status { get; init; }
        public required Priority? Priority { get; init; }
        public required int AccidentId { get; init; }
    }
}
