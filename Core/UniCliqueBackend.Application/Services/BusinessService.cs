using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Business;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public BusinessService(
            IBusinessRepository businessRepository,
            IUserRepository userRepository,
            IFriendshipRepository friendshipRepository)
        {
            _businessRepository = businessRepository;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public async Task<bool> CreateBusinessRequestAsync(string userId, CreateBusinessRequestDto model)
        {
            if (!Guid.TryParse(userId, out var uid)) return false;

            var existing = await _businessRepository.GetRequestByUserIdAsync(uid);
            if (existing != null && existing.Status == BusinessRequestStatus.Pending)
                return false; // Already pending

            var request = new BusinessRequest
            {
                UserId = uid,
                BusinessName = model.BusinessName,
                Description = model.Description,
                Status = BusinessRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _businessRepository.AddRequestAsync(request);
            return true;
        }

        public async Task<BusinessRequestDto?> GetMyRequestAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var uid)) return null;

            var request = await _businessRepository.GetRequestByUserIdAsync(uid);
            if (request == null) return null;

            return MapToDto(request);
        }

        public async Task<IEnumerable<BusinessRequestDto>> GetPendingRequestsAsync()
        {
            var requests = await _businessRepository.GetPendingRequestsAsync();
            return requests.Select(MapToDto);
        }

        public async Task<bool> ApproveRequestAsync(Guid requestId, string adminId)
        {
            var request = await _businessRepository.GetRequestByIdAsync(requestId);
            if (request == null || request.Status != BusinessRequestStatus.Pending) return false;

            request.Status = BusinessRequestStatus.Approved;
            request.ProcessedAt = DateTime.UtcNow;
            request.AdminResponse = "Approved by Admin.";

            await _businessRepository.UpdateRequestAsync(request);

            // Update User Role to Business
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user != null)
            {
                user.Role = RoleType.Business;
                await _userRepository.UpdateAsync(user);
            }

            return true;
        }

        public async Task<bool> RejectRequestAsync(Guid requestId, string adminId, string reason)
        {
            var request = await _businessRepository.GetRequestByIdAsync(requestId);
            if (request == null || request.Status != BusinessRequestStatus.Pending) return false;

            request.Status = BusinessRequestStatus.Rejected;
            request.ProcessedAt = DateTime.UtcNow;
            request.AdminResponse = reason;

            await _businessRepository.UpdateRequestAsync(request);
            return true;
        }

        public async Task<BusinessStatsDto> GetBusinessStatsAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var uid)) return new BusinessStatsDto();

            var totalEvents = await _businessRepository.GetTotalEventsAsync(uid);
            var totalParticipants = await _businessRepository.GetTotalParticipantsAsync(uid);
            var activeEvents = await _businessRepository.GetActiveEventsAsync(uid);
            
            // Friendship count as "Followers"
            var friends = await _friendshipRepository.GetFriendsAsync(uid);
            var friendCount = friends.Count();

            double avg = totalEvents > 0 ? (double)totalParticipants / totalEvents : 0;

            return new BusinessStatsDto
            {
                TotalEvents = totalEvents,
                TotalParticipants = totalParticipants,
                ActiveEvents = activeEvents,
                FriendshipCount = friendCount,
                AverageParticipantsPerEvent = Math.Round(avg, 2)
            };
        }

        private BusinessRequestDto MapToDto(BusinessRequest request)
        {
            return new BusinessRequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = request.User?.FullName ?? "Unknown",
                BusinessName = request.BusinessName,
                Description = request.Description,
                Status = request.Status,
                AdminResponse = request.AdminResponse,
                CreatedAt = request.CreatedAt
            };
        }
    }
}
