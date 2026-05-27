namespace sicoain.shared.Entities
{
    public class BusinessEmail : BaseEntityEmail
    {
        public required int BusinessId { get; set; }

        public required Business Business { get; set; }
    }
}
