using System;
using System.ComponentModel.DataAnnotations;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Username { get; set; } 

        [Required]
        public string PasswordHash { get; set; }

        [Required, MaxLength(10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Telefon numarası 10 haneli olmalıdır.")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }


        // Rol veritabanında varsayılan olarak "User" (0) olarak atanacak (Fluent API'de yapılacak)
        [Required]
        public RoleType Role { get; set; }

        public bool IsActive { get; set; }

        public bool IsBanned { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<UserConsent> UserConsents { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
