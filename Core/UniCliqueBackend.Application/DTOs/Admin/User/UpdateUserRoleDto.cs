using System.ComponentModel.DataAnnotations;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.DTOs.Admin.User
{
    public class UpdateUserRoleDto
    {
        [Required]
        public RoleType NewRole { get; set; }
    }
}
