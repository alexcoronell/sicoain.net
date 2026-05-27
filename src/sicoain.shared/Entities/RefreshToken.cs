using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    /// <summary>
    /// Refresh token for JWT token rotation and revocation
    /// Stored in database for maximum security
    /// </summary>
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        /****************************** Token Data ******************************/
        /// <summary>
        /// Unique token (stored as hash in production)
        /// </summary>
        [Required]
        [MaxLength(200)]
        public required string Token { get; set; }

        /// <summary>
        /// ID of the associated user
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Navigation property to the user
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;


        /****************************** Expiration ******************************/
        /// <summary>
        /// Token expiration date
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }


        /****************************** Revocation ******************************/
        /// <summary>
        /// Revocation date (null if not revoked)
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// IP address from which the token was revoked
        /// </summary>
        [MaxLength(45)]
        public string? RevokedByIp { get; set; }

        /// <summary>
        /// Reason for revocation (optional)
        /// </summary>
        [MaxLength(200)]
        public string? RevokedReason { get; set; }

        /// <summary>
        /// Replaced by (ID of the new refresh token)
        /// Used for token rotation chain tracking
        /// </summary>
        public int? ReplacedByTokenId { get; set; }


        /****************************** Audit ******************************/
        /// <summary>
        /// Date the token was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// IP address from which the token was created
        /// </summary>
        [MaxLength(45)]
        public string? CreatedByIp { get; set; }


        /****************************** Computed Properties (not mapped) ******************************/
        /// <summary>
        /// Is the token revoked?
        /// </summary>
        [NotMapped]
        public bool IsRevoked => RevokedAt != null;

        /// <summary>
        /// Is the token expired?
        /// </summary>
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        /// <summary>
        /// Is the token active? (not revoked and not expired)
        /// </summary>
        [NotMapped]
        public bool IsActive => !IsRevoked && !IsExpired;

        /// <summary>
        /// Mark token as revoked
        /// </summary>
        /// <param name="ipAddress">IP address from which revocation occurred</param>
        /// <param name="reason">Optional reason for revocation</param>
        public void Revoke(string ipAddress, string? reason = null)
        {
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = ipAddress;
            RevokedReason = reason;
        }
    }
}
