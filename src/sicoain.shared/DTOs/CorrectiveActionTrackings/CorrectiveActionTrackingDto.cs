namespace sicoain.shared.DTOs.CorrectiveActionTrackings
{
    public record CorrectiveActionTrackingDto : BaseDto
    {
        public int CorrectiveActionId { get; init; }
        public string OldStatus { get; init; } = string.Empty;
        public string NewStatus { get; init; } = string.Empty;
        public DateTime TrackingDate { get; init; }
        public string Comments { get; init; } = string.Empty;
    }
}
