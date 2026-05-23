using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.CorrectiveActions
{
    public record UpdateCorrectiveActionRequest
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public DateTime? DueDate { get; init; }
        public StatusAction? Status { get; init; }
        public Priority? Priority { get; init; }
        public int? AccidentId { get; init; }
    }
}
