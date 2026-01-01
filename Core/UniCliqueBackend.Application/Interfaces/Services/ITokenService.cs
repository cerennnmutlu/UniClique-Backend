using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, DateTime expiresAt);
        string GenerateRefreshToken();

        string HashRefreshToken(string refreshToken);
    }
}
