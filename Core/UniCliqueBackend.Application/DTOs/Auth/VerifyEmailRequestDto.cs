namespace UniCliqueBackend.Application.DTOs.Auth
{
    public class VerifyEmailRequestDto
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
