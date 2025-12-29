using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Common;

namespace UniCliqueBackend.Domain.Entities
{
    public class UserRefreshToken : BaseEntity
    {
        // Token plain tutulmaz -> HASH
        [Required]
        public string TokenHash { get; set; } = "";

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked => RevokedAt.HasValue;

        // Rotation: eski token yenilenince yeni token hash'i burada tutulur.
        public string? ReplacedByTokenHash { get; set; }

        // Audit / güvenlik
        [MaxLength(100)]
        public string? DeviceId { get; set; }

        [MaxLength(300)]
        public string? UserAgent { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
