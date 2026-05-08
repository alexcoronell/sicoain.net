using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace sicoain.shared.Entities
{
    public class User : IdentityUser<int>
    {
        [Required, MinLength(8), MaxLength(100)]
        public required string FullName { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
