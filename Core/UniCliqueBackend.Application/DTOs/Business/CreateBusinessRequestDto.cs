using System.ComponentModel.DataAnnotations;

namespace UniCliqueBackend.Application.DTOs.Business
{
    public class CreateBusinessRequestDto
    {
        [Required, MaxLength(100)]
        public string BusinessName { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
