using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Persistence.Contexts;

namespace UniCliqueBackend.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<User> GetByIdAsync(int id)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public Task<User> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await _db.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User> AddAsync(User user)
        {
            var entry = await _db.Users.AddAsync(user);
            return entry.Entity;
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}


