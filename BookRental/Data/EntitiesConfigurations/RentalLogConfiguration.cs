using BookRental.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRental.Data.EntitiesConfigurations;

public class RentalLogConfiguration : IEntityTypeConfiguration<RentalLog>
{
    public void Configure(EntityTypeBuilder<RentalLog> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasOne(b => b.Rental)
               .WithMany(b => b.RentalLogs)
               .HasForeignKey(b => b.RentalId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(b => b.RentalStatus)
                .WithMany()
                .HasForeignKey(b => b.RentalStatusId)
                .IsRequired();

        builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
    }
}
