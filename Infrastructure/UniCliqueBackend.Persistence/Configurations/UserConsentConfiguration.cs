using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Persistence.Configurations
{
    public class UserConsentConfiguration : IEntityTypeConfiguration<UserConsent>
    {
        public void Configure(EntityTypeBuilder<UserConsent> builder)
        {
            builder.ToTable("UserConsents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DocumentVersion)
                   .HasMaxLength(50);

            // Aynı kullanıcı aynı consent'i 1 kez kabul edebilir
            builder.HasIndex(x => new { x.UserId, x.ConsentType })
                   .IsUnique();

            // Soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
