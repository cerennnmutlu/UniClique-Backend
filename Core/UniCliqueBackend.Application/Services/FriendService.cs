using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Friendship;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;

        public FriendService(IFriendshipRepository friendshipRepository, IUserRepository userRepository)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> SendFriendRequestAsync(string userId, string targetUserId)
        {
            if (!Guid.TryParse(userId, out var requesterId) || !Guid.TryParse(targetUserId, out var addresseeId))
                return false;

            if (requesterId == addresseeId) return false;

            var existing = await _friendshipRepository.GetFriendshipAsync(requesterId, addresseeId);
            if (existing != null) return false; // Already friends or pending

            var friendship = new Friendship
            {
                RequesterId = requesterId,
                AddresseeId = addresseeId,
                Status = FriendshipStatus.Pending
            };

            await _friendshipRepository.AddAsync(friendship);
            return true;
        }

        public async Task<bool> AcceptFriendRequestAsync(string userId, Guid requestId)
        {
            if (!Guid.TryParse(userId, out var currentUserId)) return false;

            var friendship = await _friendshipRepository.GetByIdAsync(requestId);
            if (friendship == null) return false;

            if (friendship.AddresseeId != currentUserId) return false; // Only addressee can accept

            friendship.Status = FriendshipStatus.Accepted;
            friendship.AcceptedAt = DateTime.UtcNow;

            await _friendshipRepository.UpdateAsync(friendship);
            
            // TODO: Update Interaction Score for both users
            
            return true;
        }

        public async Task<bool> RejectFriendRequestAsync(string userId, Guid requestId)
        {
            if (!Guid.TryParse(userId, out var currentUserId)) return false;

            var friendship = await _friendshipRepository.GetByIdAsync(requestId);
            if (friendship == null) return false;

            if (friendship.AddresseeId != currentUserId) return false;

            friendship.Status = FriendshipStatus.Rejected;
            await _friendshipRepository.DeleteAsync(friendship); // Or keep as rejected? Usually delete for re-request.
            
            return true;
        }

        public async Task<bool> RemoveFriendAsync(string userId, string friendId)
        {
             if (!Guid.TryParse(userId, out var currentUserId) || !Guid.TryParse(friendId, out var targetId))
                return false;

            var friendship = await _friendshipRepository.GetFriendshipAsync(currentUserId, targetId);
            if (friendship == null) return false;

            await _friendshipRepository.DeleteAsync(friendship);
            return true;
        }

        public async Task<IEnumerable<FriendDto>> GetFriendsAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var currentUserId)) return Enumerable.Empty<FriendDto>();

            var friends = await _friendshipRepository.GetFriendsAsync(currentUserId);
            return friends.Select(f => new FriendDto
            {
                UserId = f.Id,
                FullName = f.FullName,
                Username = f.Username,
                ProfilePhotoUrl = f.ProfilePhotoUrl,
                University = f.University,
                Department = f.Department
            });
        }

        public async Task<IEnumerable<FriendRequestDto>> GetPendingRequestsAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var currentUserId)) return Enumerable.Empty<FriendRequestDto>();

            var requests = await _friendshipRepository.GetFriendRequestsAsync(currentUserId, incoming: true);
            return requests.Select(f => new FriendRequestDto
            {
                Id = f.Id,
                UserId = f.RequesterId,
                FullName = f.Requester.FullName,
                ProfilePhotoUrl = f.Requester.ProfilePhotoUrl,
                Status = f.Status,
                CreatedAt = f.CreatedAt
            });
        }

        public async Task<IEnumerable<FriendDto>> SearchUsersAsync(string query, string currentUserId)
        {
            var users = await _userRepository.SearchUsersAsync(query);
            
            // Filter out current user from results
            return users
                .Where(u => u.Id.ToString() != currentUserId)
                .Select(u => new FriendDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Username = u.Username,
                    ProfilePhotoUrl = u.ProfilePhotoUrl,
                    University = u.University,
                    Department = u.Department
                });
        }
    }
}
