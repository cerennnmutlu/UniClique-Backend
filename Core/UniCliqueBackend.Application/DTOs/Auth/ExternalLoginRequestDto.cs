namespace UniCliqueBackend.Application.DTOs.Auth
{
    public class ExternalLoginRequestDto
    {
        public string Provider { get; set; } = null!;
        public string ProviderUserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
    }
}
