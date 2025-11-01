using System.Threading.Tasks;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsUsernameTakenAsync(string username);
        Task<User> AddAsync(User user);
        Task SaveChangesAsync();
    }
}


