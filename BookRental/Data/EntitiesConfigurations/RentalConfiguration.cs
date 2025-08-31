using BookRental.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRental.Data.EntitiesConfigurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasOne(b => b.Book)
               .WithMany()
               .HasForeignKey(b => b.BookId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(b => b.User)
               .WithMany(t => t.Rentals)
               .HasForeignKey(b => b.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);

        builder.Property(b => b.RentedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(b => b.RentalStatus)
               .WithMany()
               .HasForeignKey(b => b.RentalStatusId)
               .IsRequired();
    }
}