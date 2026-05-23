using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class BranchPhone : BaseEntityPhone
    {
        public required int BranchId { get; set; }

        public required Branch Branch { get; set; }
    }
}
