using System.ComponentModel.DataAnnotations;

namespace UniCliqueBackend.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime BirthDate { get; set; }

        public string Password { get; set; } = null!;

        // Consents
        public bool AcceptKvkk { get; set; }
        public bool AcceptTerms { get; set; }
        public bool AcceptPrivacy { get; set; }
    }
}
