using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UniCliqueBackend.Domain.Common;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = ""; 

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = "";

        [Required, MaxLength(50)]
        public string Username { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = "";

        public DateTime BirthDate { get; set; }

        public RoleType Role { get; set; } = RoleType.User;

        public bool IsEmailVerified { get; set; } = false;
        public DateTime? EmailVerifiedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsBanned { get; set; } = false;

        public DateTime? LastLoginAt { get; set; }

        // Navigation
        public ICollection<UserConsent> UserConsents { get; set; } = new List<UserConsent>();
        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
        public ICollection<UserVerificationCode> VerificationCodes { get; set; } = new List<UserVerificationCode>();
        public ICollection<UserExternalLogin> ExternalLogins { get; set; } = new List<UserExternalLogin>();
    }
}
