using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Auth;

namespace UniCliqueBackend.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    }
}


