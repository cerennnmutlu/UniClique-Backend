using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Interfaces.Repositories
{
    public interface IUserConsentRepository
    {
        Task AddRangeAsync(IEnumerable<UserConsent> consents);
        Task SaveChangesAsync();
    }
}


