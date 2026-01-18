using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Common;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Domain.Entities
{
    public class BusinessRequest : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required, MaxLength(100)]
        public string BusinessName { get; set; } = "";

        [MaxLength(500)]
        public string Description { get; set; } = "";

        public BusinessRequestStatus Status { get; set; } = BusinessRequestStatus.Pending;

        [MaxLength(500)]
        public string? AdminResponse { get; set; }

        public DateTime? ProcessedAt { get; set; }
    }
}
