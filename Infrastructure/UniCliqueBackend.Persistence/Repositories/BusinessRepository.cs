using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;
using UniCliqueBackend.Persistence.Contexts;

namespace UniCliqueBackend.Persistence.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly AppDbContext _context;

        public BusinessRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRequestAsync(BusinessRequest request)
        {
            await _context.BusinessRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRequestAsync(BusinessRequest request)
        {
            _context.BusinessRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<BusinessRequest?> GetRequestByIdAsync(Guid id)
        {
            return await _context.BusinessRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<BusinessRequest?> GetRequestByUserIdAsync(Guid userId)
        {
            return await _context.BusinessRequests
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task<IEnumerable<BusinessRequest>> GetPendingRequestsAsync()
        {
            return await _context.BusinessRequests
                .Include(r => r.User)
                .Where(r => r.Status == BusinessRequestStatus.Pending)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalEventsAsync(Guid ownerId)
        {
            return await _context.Events.CountAsync(e => e.OwnerId == ownerId);
        }

        public async Task<int> GetTotalParticipantsAsync(Guid ownerId)
        {
            // Sum of current participants in all events owned by this user
            return await _context.Events
                .Where(e => e.OwnerId == ownerId)
                .SumAsync(e => e.CurrentParticipantsCount);
        }

        public async Task<int> GetActiveEventsAsync(Guid ownerId)
        {
            return await _context.Events
                .CountAsync(e => e.OwnerId == ownerId && !e.IsCancelled && e.EndDate > DateTime.UtcNow);
        }
    }
}
