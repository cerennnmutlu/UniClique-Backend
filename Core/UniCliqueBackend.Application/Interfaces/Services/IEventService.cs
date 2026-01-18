using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Event;

namespace UniCliqueBackend.Application.Interfaces.Services
{
    public interface IEventService
    {
        Task<EventDto?> GetEventByIdAsync(Guid eventId, string currentUserId);
        Task<IEnumerable<EventDto>> GetAllEventsAsync(string currentUserId);
        Task<IEnumerable<EventDto>> GetMyEventsAsync(string currentUserId); // Created by me
        Task<IEnumerable<EventDto>> GetJoinedEventsAsync(string currentUserId);
        
        Task<EventDto?> CreateEventAsync(CreateEventDto model, string userId);
        Task<bool> UpdateEventAsync(Guid eventId, CreateEventDto model, string userId);
        Task<bool> CancelEventAsync(Guid eventId, string userId);
        
        Task<bool> JoinEventAsync(Guid eventId, string userId);
        Task<bool> LeaveEventAsync(Guid eventId, string userId);
    }
}
