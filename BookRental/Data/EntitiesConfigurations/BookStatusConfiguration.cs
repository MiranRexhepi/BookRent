using BookRental.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRental.Data.EntitiesConfigurations;

public class BookStatusConfiguration : IEntityTypeConfiguration<BookStatus>
{
    public void Configure(EntityTypeBuilder<BookStatus> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasData(
            new BookStatus { Id = 1, Name = Enums.BookStatus.Available.ToString() },
            new BookStatus { Id = 2, Name = Enums.BookStatus.Rented.ToString() }
            );
    }
}
