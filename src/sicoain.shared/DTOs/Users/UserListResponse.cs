namespace sicoain.shared.DTOs.Users
{
    public record UserListResponse
    {
        public List<UserDto> Items { get; init; } = new();
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
