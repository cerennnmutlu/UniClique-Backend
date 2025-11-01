using System;
using System.ComponentModel.DataAnnotations;

namespace UniCliqueBackend.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Telefon numarası 10 haneli olmalıdır.")]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        // Consents
        [Required]
        [System.ComponentModel.DataAnnotations.Range(typeof(bool), "true", "true", ErrorMessage = "KVKK onayı gereklidir.")]
        public bool KvkkAccepted { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.Range(typeof(bool), "true", "true", ErrorMessage = "Gizlilik Politikası onayı gereklidir.")]
        public bool PrivacyPolicyAccepted { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.Range(typeof(bool), "true", "true", ErrorMessage = "Kullanım Şartları onayı gereklidir.")]
        public bool TermsOfServiceAccepted { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        public string UsernameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}


