using Microsoft.EntityFrameworkCore;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserConsent> UserConsents { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USERS
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.FullName)
                      .IsRequired();

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(u => u.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.Property(u => u.BirthDate)
                      .IsRequired()
                      .HasColumnType("date");

                entity.Property(u => u.Role)
                      .HasDefaultValue(RoleType.User)
                      .IsRequired();

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                entity.Property(u => u.IsBanned)
                      .HasDefaultValue(false);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Unique index’ler
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();

                // Navigation ilişkileri
                entity.HasMany(u => u.UserConsents)
                      .WithOne(uc => uc.User)
                      .HasForeignKey(uc => uc.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.RefreshTokens)
                      .WithOne(rt => rt.User)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // USER CONSENTS
            modelBuilder.Entity<UserConsent>(entity =>
            {
                entity.ToTable("user_consents");

                entity.HasKey(uc => uc.Id);

                entity.Property(uc => uc.ConsentType)
                      .IsRequired();

                entity.Property(uc => uc.IsAccepted)
                      .IsRequired();

                entity.Property(uc => uc.AcceptedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Aynı kullanıcı aynı policy'yi iki kez kaydedemesin
                entity.HasIndex(uc => new { uc.UserId, uc.ConsentType }).IsUnique();
            });

            // REFRESH TOKENS
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens");

                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.Token)
                      .IsRequired();

                entity.Property(rt => rt.Expiration)
                      .IsRequired()
                      .HasDefaultValueSql("CURRENT_TIMESTAMP + interval '30 days'");

                entity.Property(rt => rt.IsRevoked)
                      .HasDefaultValue(false);

                entity.Property(rt => rt.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(rt => rt.Token).IsUnique();
            });

            // ===========================
            // 🌱 SEED DATA (örnek kayıtlar)
            // ===========================
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Admin User",
                    Email = "admin@uniclique.com",
                    Username = "admin",
                    PasswordHash = "admin123",
                    PhoneNumber = "5555555555",
                    BirthDate = new DateTime(1995, 5, 10),
                    Role = RoleType.Admin,
                    IsActive = true,
                    IsBanned = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    FullName = "Business Owner",
                    Email = "business@uniclique.com",
                    Username = "business1",
                    PasswordHash = "biz123",
                    PhoneNumber = "5551112233",
                    BirthDate = new DateTime(1990, 4, 14),
                    Role = RoleType.Business,
                    IsActive = true,
                    IsBanned = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 3,
                    FullName = "Regular User",
                    Email = "user@uniclique.com",
                    Username = "user1",
                    PasswordHash = "user123",
                    PhoneNumber = "5552223344",
                    BirthDate = new DateTime(2000, 1, 20),
                    Role = RoleType.User,
                    IsActive = true,
                    IsBanned = false,
                    CreatedAt = DateTime.UtcNow
                }
            );

            modelBuilder.Entity<UserConsent>().HasData(
                new UserConsent { Id = 1, UserId = 1, ConsentType = ConsentType.KVKK, IsAccepted = true },
                new UserConsent { Id = 2, UserId = 1, ConsentType = ConsentType.PrivacyPolicy, IsAccepted = true },
                new UserConsent { Id = 3, UserId = 2, ConsentType = ConsentType.KVKK, IsAccepted = true },
                new UserConsent { Id = 4, UserId = 3, ConsentType = ConsentType.TermsOfService, IsAccepted = true }
            );

            modelBuilder.Entity<RefreshToken>().HasData(
                new RefreshToken
                {
                    Id = 1,
                    Token = "sample_refresh_token_admin",
                    UserId = 1,
                    Expiration = DateTime.UtcNow.AddDays(30),
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new RefreshToken
                {
                    Id = 2,
                    Token = "sample_refresh_token_user",
                    UserId = 3,
                    Expiration = DateTime.UtcNow.AddDays(30),
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
