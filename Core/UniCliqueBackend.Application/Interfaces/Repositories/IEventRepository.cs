using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Interfaces.Repositories
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid id);
        Task<IEnumerable<Event>> GetAllAsync(); // Maybe with filters later
        Task<IEnumerable<Event>> GetEventsByOwnerAsync(Guid ownerId);
        
        Task AddAsync(Event evt);
        Task UpdateAsync(Event evt);
        Task DeleteAsync(Event evt);
        
        Task AddParticipantAsync(EventParticipant participant);
        Task RemoveParticipantAsync(EventParticipant participant);
        Task<EventParticipant?> GetParticipantAsync(Guid eventId, Guid userId);
        Task<IEnumerable<EventParticipant>> GetParticipantsAsync(Guid eventId);
        
        Task<IEnumerable<Event>> GetJoinedEventsAsync(Guid userId);
    }
}
