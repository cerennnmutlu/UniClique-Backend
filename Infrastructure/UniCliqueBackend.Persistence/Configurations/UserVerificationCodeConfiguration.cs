using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Persistence.Configurations
{
    public class UserVerificationCodeConfiguration 
        : IEntityTypeConfiguration<UserVerificationCode>
    {
        public void Configure(EntityTypeBuilder<UserVerificationCode> builder)
        {
            builder.ToTable("UserVerificationCodes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CodeHash)
                   .IsRequired();

            builder.Property(x => x.SentTo)
                   .IsRequired()
                   .HasMaxLength(255);

            // OTP lookup ve temizlik için index
            builder.HasIndex(x => new
            {
                x.UserId,
                x.Purpose,
                x.ExpiresAt
            });

            // Soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
