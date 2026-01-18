using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Persistence.Contexts;

namespace UniCliqueBackend.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Owner)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Owner)
                .Where(e => !e.IsCancelled && e.EndDate > DateTime.UtcNow) // Show only active events by default
                .OrderBy(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByOwnerAsync(Guid ownerId)
        {
            return await _context.Events
                .Include(e => e.Owner)
                .Where(e => e.OwnerId == ownerId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Event evt)
        {
            await _context.Events.AddAsync(evt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event evt)
        {
            _context.Events.Update(evt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Event evt)
        {
            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddParticipantAsync(EventParticipant participant)
        {
            await _context.EventParticipants.AddAsync(participant);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveParticipantAsync(EventParticipant participant)
        {
            _context.EventParticipants.Remove(participant);
            await _context.SaveChangesAsync();
        }

        public async Task<EventParticipant?> GetParticipantAsync(Guid eventId, Guid userId)
        {
            return await _context.EventParticipants
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId);
        }

        public async Task<IEnumerable<EventParticipant>> GetParticipantsAsync(Guid eventId)
        {
            return await _context.EventParticipants
                .Include(ep => ep.User)
                .Where(ep => ep.EventId == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetJoinedEventsAsync(Guid userId)
        {
            return await _context.EventParticipants
                .Where(ep => ep.UserId == userId)
                .Include(ep => ep.Event)
                    .ThenInclude(e => e.Owner)
                .Select(ep => ep.Event)
                .ToListAsync();
        }
    }
}
