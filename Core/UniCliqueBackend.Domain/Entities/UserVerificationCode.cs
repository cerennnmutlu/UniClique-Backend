using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Common;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Domain.Entities
{
    public class UserVerificationCode : BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        [Required]
        public VerificationPurpose Purpose { get; set; }

        // OTP plain tutulmaz -> HASH
        [Required]
        public string CodeHash { get; set; } = "";

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }

        // brute-force kontrolü için
        public int AttemptCount { get; set; } = 0;
        public DateTime? LastAttemptAt { get; set; }

        [Required, MaxLength(255)]
        public string SentTo { get; set; } = ""; // email

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
