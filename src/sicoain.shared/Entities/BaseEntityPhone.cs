using System.ComponentModel.DataAnnotations.Schema;
using sicoain.shared.Enums;

namespace sicoain.shared.Entities
{
    public abstract class BaseEntityPhone : BaseEntity
    {
        public required string Phone { get; set; }

        [Column("phone_type")]
        public required PhoneType PhoneType { get; set; }
    }
}
