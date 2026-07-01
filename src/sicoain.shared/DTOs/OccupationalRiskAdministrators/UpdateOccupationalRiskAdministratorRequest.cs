namespace sicoain.shared.DTOs.OccupationalRiskAdministrators
{
    public record UpdateOccupationalRiskAdministratorRequest
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }

        public List<string>? Emails { get; init; }
        public List<string>? Phones { get; init; }
    }
}
