using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;
using UniCliqueBackend.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace UniCliqueBackend.Persistence.Seed
{
    public static class AppDbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            
            // DB hazır mı?
            await context.Database.MigrateAsync();

            // ⚠️ Global check removed to allow adding admins even if other users exist
            // if (await context.Users.AnyAsync()) return;

            // 🔐 1. Default System Admin
            if (!await context.Users.AnyAsync(u => u.Email == "admin@uniclique.dev"))
            {
                var adminUser = new User
                {
                    FullName = "System Admin",
                    Email = "admin@uniclique.dev",
                    Username = "admin",
                    PhoneNumber = "+905555555555",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1995, 1, 1), DateTimeKind.Utc),
                    Role = RoleType.Admin,
                    IsEmailVerified = true,
                    EmailVerifiedAt = DateTime.UtcNow,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
                };
                context.Users.Add(adminUser);
                AddConsents(context, adminUser);
            }

            // 🔐 2. Custom Admin (Ceren)
            if (!await context.Users.AnyAsync(u => u.Email == "ceren.dev.uniclique@uniclique.dev"))
            {
                var cerenAdmin = new User
                {
                    FullName = "Ceren Admin",
                    Email = "ceren.dev.uniclique@uniclique.dev",
                    Username = "ceren_admin",
                    PhoneNumber = "+905555555556", // Different phone
                    BirthDate = DateTime.SpecifyKind(new DateTime(1995, 1, 1), DateTimeKind.Utc),
                    Role = RoleType.Admin,
                    IsEmailVerified = true,
                    EmailVerifiedAt = DateTime.UtcNow,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123123Aa.")
                };
                context.Users.Add(cerenAdmin);
                AddConsents(context, cerenAdmin);
            }

            // ✅ Consentler (3’ü de kabul)
            await context.SaveChangesAsync();
        }

        private static void AddConsents(AppDbContext context, User user)
        {
            context.UserConsents.AddRange(
                new UserConsent { User = user, ConsentType = ConsentType.KVKK, IsAccepted = true, DocumentVersion = "v1" },
                new UserConsent { User = user, ConsentType = ConsentType.PrivacyPolicy, IsAccepted = true, DocumentVersion = "v1" },
                new UserConsent { User = user, ConsentType = ConsentType.TermsOfService, IsAccepted = true, DocumentVersion = "v1" }
            );
        }


    }
}
