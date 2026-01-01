namespace UniCliqueBackend.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        public string EmailOrUsername { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
