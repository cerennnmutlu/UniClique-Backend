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

            // 🔴 Eğer User varsa seed tekrar çalışmaz
            if (await context.Users.AnyAsync())
                return;

            // 🔐 Admin User
            var adminUser = new User
            {
                FullName = "System Admin",
                Email = "admin@uniclique.dev",
                Username = "admin",
                PhoneNumber = "+905555555555",
                BirthDate = DateTime.SpecifyKind(
                new DateTime(1995, 1, 1),
                DateTimeKind.Utc
         ),
                Role = RoleType.Admin,
                IsEmailVerified = true,
                EmailVerifiedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };

            context.Users.Add(adminUser);

            // ✅ Consentler (3’ü de kabul)
            context.UserConsents.AddRange(
                new UserConsent
                {
                    User = adminUser,
                    ConsentType = ConsentType.KVKK,
                    IsAccepted = true,
                    DocumentVersion = "v1"
                },
                new UserConsent
                {
                    User = adminUser,
                    ConsentType = ConsentType.PrivacyPolicy,
                    IsAccepted = true,
                    DocumentVersion = "v1"
                },
                new UserConsent
                {
                    User = adminUser,
                    ConsentType = ConsentType.TermsOfService,
                    IsAccepted = true,
                    DocumentVersion = "v1"
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
