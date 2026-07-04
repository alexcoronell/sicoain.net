using sicoain.shared.Interfaces;

namespace sicoain.shared.Entities
{
    public abstract class BaseEntityEmail : BaseEntity, IHasIsMain
    {
        public required string Email { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
