using Microsoft.EntityFrameworkCore;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Persistence.Contexts;

namespace UniCliqueBackend.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRefreshTokenAsync(Guid userId, UserRefreshToken token)
        {
            token.UserId = userId;
            _context.UserRefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserRefreshToken?> GetRefreshTokenAsync(string tokenHash)
        {
            return await _context.UserRefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.IsDeleted);
        }

        public async Task RevokeRefreshTokenAsync(UserRefreshToken token, string? replacedByTokenHash)
        {
            var entity = await _context.UserRefreshTokens.FirstOrDefaultAsync(x => x.Id == token.Id);
            if (entity != null)
            {
                entity.RevokedAt = DateTime.UtcNow;
                entity.ReplacedByTokenHash = replacedByTokenHash;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsAsync(string email, string username)
        {
            return await _context.Users
                .AnyAsync(x => x.Email == email || x.Username == username);
        }

        public async Task<User?> GetByExternalLoginAsync(string provider, string providerUserId)
        {
            var login = await _context.UserExternalLogins
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Provider == provider && x.ProviderUserId == providerUserId && !x.IsDeleted);
            return login?.User;
        }

        public async Task AddExternalLoginAsync(Guid userId, UserExternalLogin externalLogin)
        {
            externalLogin.UserId = userId;
            _context.UserExternalLogins.Add(externalLogin);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PhoneExistsAsync(string phoneE164, string phoneLocal)
        {
            return await _context.Users.AnyAsync(x =>
                x.PhoneNumber == phoneE164 || x.PhoneNumber == phoneLocal
            );
        }
    }
}
