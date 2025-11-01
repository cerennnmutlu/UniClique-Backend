using System.Threading.Tasks;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddAsync(RefreshToken token);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task RevokeAsync(RefreshToken token);
        Task SaveChangesAsync();
    }
}


