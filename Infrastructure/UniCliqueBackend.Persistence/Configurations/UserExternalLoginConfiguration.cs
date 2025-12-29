using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Persistence.Configurations
{
    public class UserExternalLoginConfiguration 
        : IEntityTypeConfiguration<UserExternalLogin>
    {
        public void Configure(EntityTypeBuilder<UserExternalLogin> builder)
        {
            builder.ToTable("UserExternalLogins");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Provider)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(x => x.ProviderUserId)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.ProviderEmail)
                   .HasMaxLength(255);

            // Aynı provider + provider user id tek olmalı
            builder.HasIndex(x => new
            {
                x.Provider,
                x.ProviderUserId
            }).IsUnique();

            // Soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
