using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Common;

namespace UniCliqueBackend.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        [Required]
        public string UserId { get; set; } = ""; // Action performed by

        public string? TargetUserId { get; set; } // Action performed on (optional)

        [Required]
        public string Action { get; set; } = ""; // e.g., "ROLE_CHANGE", "USER_BAN"

        public string? Details { get; set; } // JSON or text details

        public string? IpAddress { get; set; }
    }
}
