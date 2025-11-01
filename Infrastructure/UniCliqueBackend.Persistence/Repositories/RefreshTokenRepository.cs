using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Persistence.Contexts;

namespace UniCliqueBackend.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _db;

        public RefreshTokenRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RefreshToken> AddAsync(RefreshToken token)
        {
            var entry = await _db.RefreshTokens.AddAsync(token);
            return entry.Entity;
        }

        public Task<RefreshToken> GetByTokenAsync(string token)
        {
            return _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
        }

        public Task RevokeAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            _db.RefreshTokens.Update(token);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}


