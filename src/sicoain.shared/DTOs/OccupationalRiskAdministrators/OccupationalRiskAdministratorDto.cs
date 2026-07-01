namespace sicoain.shared.DTOs.OccupationalRiskAdministrators
{
    public record OccupationalRiskAdministratorDto
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
        public List<OccupationalRiskAdministratorEmailDto>? Emails { get; init; }
        public List<OccupationalRiskAdministratorPhoneDto>? Phones { get; init; }
    }
}
