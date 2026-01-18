using System;
using System.Collections.Generic;

namespace UniCliqueBackend.Application.DTOs.Event
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int CurrentParticipantsCount { get; set; }
        
        public bool IsBusinessEvent { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsJoined { get; set; } // If the current user has joined

        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string? OwnerProfilePhoto { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
