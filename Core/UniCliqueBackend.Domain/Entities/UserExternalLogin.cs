using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Common;

namespace UniCliqueBackend.Domain.Entities
{
    public class UserExternalLogin : BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        // "Google" / "Facebook"
        [Required, MaxLength(30)]
        public string Provider { get; set; } = "";

        // Provider'ın verdiği user id / sub
        [Required, MaxLength(200)]
        public string ProviderUserId { get; set; } = "";

        // opsiyonel: provider'dan gelen email
        [MaxLength(255)]
        public string? ProviderEmail { get; set; }
    }
}
