using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Domain.Entities
{
    public class UserConsent
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public ConsentType ConsentType { get; set; } // KVKK, PrivacyPolicy, TermsOfService

        [Required]
        public bool IsAccepted { get; set; }

        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
    }
}
