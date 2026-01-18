using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Common;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Domain.Entities
{
    public class Friendship : BaseEntity
    {
        [Required]
        public Guid RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public User Requester { get; set; } = null!;

        [Required]
        public Guid AddresseeId { get; set; }
        [ForeignKey("AddresseeId")]
        public User Addressee { get; set; } = null!;

        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

        public DateTime? AcceptedAt { get; set; }
    }
}
