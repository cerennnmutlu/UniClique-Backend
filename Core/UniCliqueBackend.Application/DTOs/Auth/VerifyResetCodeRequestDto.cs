namespace UniCliqueBackend.Application.DTOs.Auth
{
    public class VerifyResetCodeRequestDto
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
