using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace sicoain.shared.Entities
{
    public class User : IdentityUser<int>
    {
        [Required, MinLength(8), MaxLength(100)]
        public required string FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public int? DeletedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsActive { get; set; } = true;

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

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
