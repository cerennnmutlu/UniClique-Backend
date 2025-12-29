using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Persistence.Configurations
{
    public class UserRefreshTokenConfiguration 
        : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.ToTable("UserRefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                   .IsRequired();

            builder.Property(x => x.DeviceId)
                   .HasMaxLength(100);

            builder.Property(x => x.UserAgent)
                   .HasMaxLength(300);

            builder.Property(x => x.IpAddress)
                   .HasMaxLength(50);

            // Refresh token hızlı lookup
            builder.HasIndex(x => x.TokenHash);

            // User bazlı temizlik
            builder.HasIndex(x => new
            {
                x.UserId,
                x.ExpiresAt
            });

            // Soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
