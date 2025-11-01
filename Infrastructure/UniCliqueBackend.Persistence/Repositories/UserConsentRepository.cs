using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Persistence.Contexts;

namespace UniCliqueBackend.Persistence.Repositories
{
    public class UserConsentRepository : IUserConsentRepository
    {
        private readonly AppDbContext _db;

        public UserConsentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddRangeAsync(IEnumerable<UserConsent> consents)
        {
            await _db.UserConsents.AddRangeAsync(consents);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}


