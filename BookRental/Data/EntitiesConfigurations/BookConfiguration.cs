using BookRental.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookRental.Data.EntitiesConfigurations;
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(b => b.Author)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(b => b.Genre)
               .HasMaxLength(50);

        builder.Property(b => b.ISBN)
               .HasMaxLength(20);

        builder.HasOne(b => b.Tenant)
               .WithMany(t => t.Books)
               .HasForeignKey(b => b.TenantId)
               .IsRequired();

        builder.HasOne(b => b.BookStatus)
               .WithMany()
               .HasForeignKey(b => b.BookStatusId);
    }
}
