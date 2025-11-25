using BookRental.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRental.Data.EntitiesConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(rt => rt.UserId)
               .IsRequired()
               .HasMaxLength(450);

        builder.Property(rt => rt.ExpiresAt)
               .IsRequired();

        builder.Property(rt => rt.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(rt => rt.IsRevoked)
               .IsRequired()
               .HasDefaultValue(false);

        builder.HasOne(rt => rt.User)
               .WithMany()
               .HasForeignKey(rt => rt.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rt => rt.Token)
               .IsUnique();

        builder.HasIndex(rt => rt.UserId);
    }
}

