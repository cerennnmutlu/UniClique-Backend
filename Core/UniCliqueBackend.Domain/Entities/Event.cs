using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniCliqueBackend.Domain.Common;

namespace UniCliqueBackend.Domain.Entities
{
    public class Event : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Title { get; set; } = "";

        [Required, MaxLength(1000)]
        public string Description { get; set; } = "";

        [Required, MaxLength(100)]
        public string Category { get; set; } = ""; // E.g., Party, Workshop

        [Required, MaxLength(200)]
        public string Location { get; set; } = "";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Capacity { get; set; }
        public int CurrentParticipantsCount { get; set; } = 0;

        public bool IsBusinessEvent { get; set; } = false;
        public bool IsCancelled { get; set; } = false;

        [Required]
        public Guid OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public User Owner { get; set; } = null!;

        // Navigation
        public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
