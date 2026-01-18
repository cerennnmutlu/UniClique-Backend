using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.Interfaces.Repositories
{
    public interface IBusinessRepository
    {
        Task AddRequestAsync(BusinessRequest request);
        Task UpdateRequestAsync(BusinessRequest request);
        Task<BusinessRequest?> GetRequestByIdAsync(Guid id);
        Task<BusinessRequest?> GetRequestByUserIdAsync(Guid userId);
        Task<IEnumerable<BusinessRequest>> GetPendingRequestsAsync();
        
        // Stats helpers
        Task<int> GetTotalEventsAsync(Guid ownerId);
        Task<int> GetTotalParticipantsAsync(Guid ownerId);
        Task<int> GetActiveEventsAsync(Guid ownerId);
    }
}
