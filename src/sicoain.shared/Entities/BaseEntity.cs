using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public int? DeletedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public void UpdateTimestamps(int userId)
        {
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted(int userId)
        {
            DeletedBy = userId;
            DeletedAt = DateTime.UtcNow;
            IsDeleted = true;
        }

        public void Restore()
        {
            DeletedBy = null;
            DeletedAt = null;
            IsDeleted = false;
        }
    }
}
