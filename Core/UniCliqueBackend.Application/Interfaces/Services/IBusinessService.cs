using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Business;

namespace UniCliqueBackend.Application.Interfaces.Services
{
    public interface IBusinessService
    {
        Task<bool> CreateBusinessRequestAsync(string userId, CreateBusinessRequestDto model);
        Task<BusinessRequestDto?> GetMyRequestAsync(string userId);
        
        // Admin
        Task<IEnumerable<BusinessRequestDto>> GetPendingRequestsAsync();
        Task<bool> ApproveRequestAsync(Guid requestId, string adminId);
        Task<bool> RejectRequestAsync(Guid requestId, string adminId, string reason);
        
        // Stats
        Task<BusinessStatsDto> GetBusinessStatsAsync(string userId);
    }
}
