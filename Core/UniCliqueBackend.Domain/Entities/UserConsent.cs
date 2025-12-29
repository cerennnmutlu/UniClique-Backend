using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Common;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Domain.Entities
{
    public class UserConsent : BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        [Required]
        public ConsentType ConsentType { get; set; }

        [Required]
        public bool IsAccepted { get; set; }

        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;

        // gelecekte KVKK metni güncellenirse hangi versiyon kabul edildi?
        [MaxLength(50)]
        public string? DocumentVersion { get; set; }
    }
}

