using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class BusinessPhone : BaseEntityPhone
    {
        public required int BusinessId { get; set; }

        public required Business Business { get; set; }
    }
}
