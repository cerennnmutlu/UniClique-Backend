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
                      .IsRequired();

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

                //  Unique index’ler
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();

                //  Navigation ilişkileri
                entity.HasMany(u => u.UserConsents)
                      .WithOne(uc => uc.User)
                      .HasForeignKey(uc => uc.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.RefreshTokens)
                      .WithOne(rt => rt.User)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        
            //  USER CONSENTS
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
            });

            //  REFRESH TOKENS
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens");

                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.Token)
                      .IsRequired();

                entity.Property(rt => rt.Expiration)
                      .IsRequired();

                entity.Property(rt => rt.IsRevoked)
                      .HasDefaultValue(false);

                entity.Property(rt => rt.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                //  Aynı token iki kez eklenmesin
                entity.HasIndex(rt => rt.Token).IsUnique();
            });
        }
    }
}
