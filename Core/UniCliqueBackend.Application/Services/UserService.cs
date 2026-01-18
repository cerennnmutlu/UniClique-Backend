using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Admin.User;
using UniCliqueBackend.Application.DTOs.User;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Application.Interfaces.Security;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IAdminRepository adminRepository, IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var users = await _adminRepository.GetAllUsersAsync(pageNumber, pageSize);
            
            return users.Select(u => new UserDto
            {
                Id = u.Id.ToString(),
                FullName = u.FullName,
                Email = u.Email,
                Username = u.Username,
                Role = u.Role,
                IsEmailVerified = u.IsEmailVerified,
                IsActive = u.IsActive,
                IsBanned = u.IsBanned,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var userId)) return null;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role,
                IsEmailVerified = user.IsEmailVerified,
                IsActive = user.IsActive,
                IsBanned = user.IsBanned,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> UpdateUserRoleAsync(string id, UpdateUserRoleDto model, string adminId)
        {
            if (!Guid.TryParse(id, out var userId)) return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var oldRole = user.Role;
            user.Role = model.NewRole;

            await _userRepository.UpdateAsync(user);

            // Audit Log
            var auditLog = new AuditLog
            {
                UserId = adminId,
                TargetUserId = id,
                Action = "ROLE_CHANGE",
                Details = $"Changed role from {oldRole} to {model.NewRole}",
                CreatedAt = DateTime.UtcNow
            };
            await _adminRepository.AddAuditLogAsync(auditLog);

            return true;
        }

        public async Task<bool> UpdateUserStatusAsync(string id, UpdateUserStatusDto model, string adminId)
        {
            if (!Guid.TryParse(id, out var userId)) return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var changes = new List<string>();

            if (model.IsActive.HasValue)
            {
                user.IsActive = model.IsActive.Value;
                changes.Add($"IsActive: {model.IsActive.Value}");
            }

            if (model.IsBanned.HasValue)
            {
                user.IsBanned = model.IsBanned.Value;
                changes.Add($"IsBanned: {model.IsBanned.Value}");
            }

            if (model.IsDeleted.HasValue && model.IsDeleted.Value)
            {
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                changes.Add("Soft Deleted");
            }
            else if (model.IsDeleted.HasValue && !model.IsDeleted.Value)
            {
                user.IsDeleted = false;
                user.DeletedAt = null; // Restore
                 changes.Add("Restored");
            }

            await _userRepository.UpdateAsync(user);

            // Audit
            var auditLog = new AuditLog
            {
                UserId = adminId,
                TargetUserId = id,
                Action = "STATUS_CHANGE",
                Details = string.Join(", ", changes),
                CreatedAt = DateTime.UtcNow
            };
            await _adminRepository.AddAuditLogAsync(auditLog);

            return true;
        }
        // User Profile Methods
        public async Task<UserProfileDto?> GetUserProfileAsync(string id)
        {
            if (!Guid.TryParse(id, out var userId)) return null;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            // TODO: Fetch real stats from specialized repositories
            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Role = user.Role,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                University = user.University,
                Department = user.Department,
                Bio = user.Bio,
                InteractionScore = user.InteractionScore,
                FriendCount = 0, // Placeholder
                CreatedEventCount = 0, // Placeholder
                JoinedEventCount = 0, // Placeholder
                IsEmailVerified = user.IsEmailVerified
            };
        }

        public async Task<bool> UpdateProfileAsync(string id, UpdateProfileDto model)
        {
            if (!Guid.TryParse(id, out var userId)) return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (model.FullName != null) user.FullName = model.FullName;
            if (model.ProfilePhotoUrl != null) user.ProfilePhotoUrl = model.ProfilePhotoUrl;
            if (model.University != null) user.University = model.University;
            if (model.Department != null) user.Department = model.Department;
            if (model.Bio != null) user.Bio = model.Bio;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string id, ChangePasswordDto model)
        {
            if (!Guid.TryParse(id, out var userId)) return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!_passwordHasher.Verify(model.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = _passwordHasher.HashPassword(model.NewPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> SoftDeleteAccountAsync(string id)
        {
            if (!Guid.TryParse(id, out var userId)) return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsDeleted = true;
            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
