using BookRental.Data;
using BookRental.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Worker;

public class OverdueBookCheck(IServiceProvider services) : BackgroundService
{
    private readonly IServiceProvider _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<BookRentalContext>();

            var overdueRentals = await context.Rentals
                .Include(r => r.RentalLogs)
                .Where(x => !x.Book.IsDeleted
                            && x.RentalStatusId == (int)Enums.RentalStatus.Rented
                            && DateTime.UtcNow > x.RentedAt.AddDays(14))
                .ToListAsync(stoppingToken);

            if (overdueRentals.Count > 0)
            {
                foreach (var r in overdueRentals)
                {
                    r.RentalStatusId = (int)Enums.RentalStatus.Overdue;
                    r.RentalLogs.Add(new RentalLog
                    {
                        RentalStatusId = (int)Enums.RentalStatus.Overdue
                    });
                }

                await context.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
