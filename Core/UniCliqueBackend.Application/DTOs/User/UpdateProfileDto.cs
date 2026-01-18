using System;
using System.ComponentModel.DataAnnotations;

namespace UniCliqueBackend.Application.DTOs.User
{
    public class UpdateProfileDto
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(200)]
        public string? ProfilePhotoUrl { get; set; }
        
        [MaxLength(100)]
        public string? University { get; set; }
        
        [MaxLength(100)]
        public string? Department { get; set; }
        
        [MaxLength(500)]
        public string? Bio { get; set; }
    }
}
