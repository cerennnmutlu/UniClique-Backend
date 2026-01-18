using System;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.DTOs.Friendship
{
    public class FriendRequestDto
    {
        public Guid Id { get; set; } // Friendship ID
        public Guid UserId { get; set; } // The other person's User ID
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
