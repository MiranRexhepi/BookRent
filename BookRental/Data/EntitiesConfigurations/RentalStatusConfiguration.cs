using BookRental.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRental.Data.EntitiesConfigurations;

public class RentalStatusConfiguration : IEntityTypeConfiguration<RentalStatus>
{
    public void Configure(EntityTypeBuilder<RentalStatus> builder)
    {
        builder.HasKey(rs => rs.Id);

        builder.Property(rs => rs.Name)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasData(
            new RentalStatus { Id = 1, Name = Enums.RentalStatus.Rented.ToString() },
            new RentalStatus { Id = 2, Name = Enums.RentalStatus.Returned.ToString() },
            new RentalStatus { Id = 3, Name = Enums.RentalStatus.Overdue.ToString() });
    }
}
