using BookRental.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Data;

public class BookRentalContext(DbContextOptions<BookRentalContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<BookStatus> BookStatuses { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<RentalStatus> RentalStatuses { get; set; }
    public DbSet<RentalLog> RentalLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "Client", NormalizedName = "CLIENT" });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookRentalContext).Assembly);
    }
}