using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task AddAuditLogAsync(AuditLog log);
    }
}
