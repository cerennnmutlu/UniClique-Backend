using UniCliqueBackend.Application.DTOs.Auth;

namespace UniCliqueBackend.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequestDto request);

        Task<TokenResponseDto> LoginAsync(LoginRequestDto request);

        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);

        Task LogoutAsync(Guid userId, string refreshToken);
        Task<TokenResponseDto> ExternalLoginAsync(ExternalLoginRequestDto request);
        Task<TokenResponseDto> VerifyEmailAsync(VerifyEmailRequestDto request);
        Task ResetDatabaseAsync();
        Task<bool> DeleteUserByEmailAsync(string email);
        Task ForgotPasswordStartAsync(ForgotPasswordStartRequestDto request);
        Task VerifyPasswordResetCodeAsync(VerifyResetCodeRequestDto request);
        Task ResetPasswordWithCodeAsync(ResetPasswordWithCodeRequestDto request);
    }
}
