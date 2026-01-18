using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Event;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;

        public EventService(IEventRepository eventRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }

        public async Task<EventDto?> GetEventByIdAsync(Guid eventId, string currentUserId)
        {
             var evt = await _eventRepository.GetByIdAsync(eventId);
             if (evt == null) return null;

             return MapToDto(evt, currentUserId);
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync(string currentUserId)
        {
            var events = await _eventRepository.GetAllAsync();
            return events.Select(e => MapToDto(e, currentUserId));
        }

        public async Task<IEnumerable<EventDto>> GetMyEventsAsync(string currentUserId)
        {
            if (!Guid.TryParse(currentUserId, out var userId)) return Enumerable.Empty<EventDto>();
            
            var events = await _eventRepository.GetEventsByOwnerAsync(userId);
            return events.Select(e => MapToDto(e, currentUserId));
        }

        public async Task<IEnumerable<EventDto>> GetJoinedEventsAsync(string currentUserId)
        {
             if (!Guid.TryParse(currentUserId, out var userId)) return Enumerable.Empty<EventDto>();
             
             var events = await _eventRepository.GetJoinedEventsAsync(userId);
             return events.Select(e => MapToDto(e, currentUserId));
        }

        public async Task<EventDto?> CreateEventAsync(CreateEventDto model, string userId)
        {
            if (!Guid.TryParse(userId, out var ownerId)) return null;

            var user = await _userRepository.GetByIdAsync(ownerId);
            if (user == null) return null;

            var evt = new Event
            {
                Title = model.Title,
                Description = model.Description,
                Category = model.Category,
                Location = model.Location,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Capacity = model.Capacity,
                OwnerId = ownerId,
                IsBusinessEvent = user.Role == RoleType.Business,
                CreatedAt = DateTime.UtcNow
            };

            await _eventRepository.AddAsync(evt);
            
            // Add owner as a participant automatically? Or kept separate? Usually owner is implied participant.
            // Let's add owner as participant for simplicity in queries if "GetJoinedEvents" includes created ones?
            // Requirement says "Kendi oluşturduğu etkinliği düzenleme", "Kendi etkinliğine gelen katılımcıları görme".
            // Implementation logic: Owner is strictly Owner.
            
            // Update Interaction Score
            user.InteractionScore += 10; // Simple gamification logic
            await _userRepository.UpdateAsync(user);

            return MapToDto(evt, userId);
        }

        public async Task<bool> UpdateEventAsync(Guid eventId, CreateEventDto model, string userId)
        {
            if (!Guid.TryParse(userId, out var ownerId)) return false;

            var evt = await _eventRepository.GetByIdAsync(eventId);
            if (evt == null) return false;

            if (evt.OwnerId != ownerId) return false; // Not the owner

            evt.Title = model.Title;
            evt.Description = model.Description;
            evt.Category = model.Category;
            evt.Location = model.Location;
            evt.StartDate = model.StartDate;
            evt.EndDate = model.EndDate;
            evt.Capacity = model.Capacity;
            
            await _eventRepository.UpdateAsync(evt);
            return true;
        }

        public async Task<bool> CancelEventAsync(Guid eventId, string userId)
        {
             if (!Guid.TryParse(userId, out var ownerId)) return false;

             var evt = await _eventRepository.GetByIdAsync(eventId);
             if (evt == null) return false;

             if (evt.OwnerId != ownerId) return false;

             evt.IsCancelled = true;
             await _eventRepository.UpdateAsync(evt);
             return true;
        }

        public async Task<bool> JoinEventAsync(Guid eventId, string userId)
        {
            if (!Guid.TryParse(userId, out var participantId)) return false;
            
            var evt = await _eventRepository.GetByIdAsync(eventId);
            if (evt == null || evt.IsCancelled) return false;
            
            if (evt.IsBusinessEvent && evt.EndDate < DateTime.UtcNow) return false; // Ended

            if (evt.CurrentParticipantsCount >= evt.Capacity) return false; // Full

            var existing = await _eventRepository.GetParticipantAsync(eventId, participantId);
            if (existing != null) return false; // Already joined

            var participant = new EventParticipant
            {
                EventId = eventId,
                UserId = participantId,
                JoinedAt = DateTime.UtcNow
            };

            await _eventRepository.AddParticipantAsync(participant);
            
            evt.CurrentParticipantsCount++;
            await _eventRepository.UpdateAsync(evt);

            // Gamification
            var user = await _userRepository.GetByIdAsync(participantId);
            if (user != null)
            {
                user.InteractionScore += 5; 
                await _userRepository.UpdateAsync(user);
            }

            return true;
        }

        public async Task<bool> LeaveEventAsync(Guid eventId, string userId)
        {
            if (!Guid.TryParse(userId, out var participantId)) return false;

            var participant = await _eventRepository.GetParticipantAsync(eventId, participantId);
            if (participant == null) return false;

            await _eventRepository.RemoveParticipantAsync(participant);

            var evt = await _eventRepository.GetByIdAsync(eventId);
            if (evt != null)
            {
                evt.CurrentParticipantsCount--;
                await _eventRepository.UpdateAsync(evt);
            }

            return true;
        }

        private EventDto MapToDto(Event evt, string currentUserId)
        {
             // Check if current user joined
             var isJoined = evt.Participants.Any(p => p.UserId.ToString() == currentUserId);

             return new EventDto
             {
                 Id = evt.Id,
                 Title = evt.Title,
                 Description = evt.Description,
                 Category = evt.Category,
                 Location = evt.Location,
                 StartDate = evt.StartDate,
                 EndDate = evt.EndDate,
                 Capacity = evt.Capacity,
                 CurrentParticipantsCount = evt.CurrentParticipantsCount,
                 IsBusinessEvent = evt.IsBusinessEvent,
                 IsCancelled = evt.IsCancelled,
                 IsJoined = isJoined,
                 OwnerId = evt.OwnerId,
                 OwnerName = evt.Owner != null ? evt.Owner.FullName : "Unknown",
                 OwnerProfilePhoto = evt.Owner != null ? evt.Owner.ProfilePhotoUrl : null,
                 CreatedAt = evt.CreatedAt
             };
        }
    }
}
