using System.ComponentModel.DataAnnotations.Schema;
using sicoain.shared.Enums;
using sicoain.shared.Interfaces;

namespace sicoain.shared.Entities
{
    public abstract class BaseEntityPhone : BaseEntity, IHasIsMain
    {
        public required string Phone { get; set; }

        [Column("phone_type")]
        public required PhoneType PhoneType { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
