using System.ComponentModel.DataAnnotations;

namespace UniCliqueBackend.Application.DTOs.Admin.User
{
    public class UpdateUserStatusDto
    {
        public bool? IsActive { get; set; }
        public bool? IsBanned { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
