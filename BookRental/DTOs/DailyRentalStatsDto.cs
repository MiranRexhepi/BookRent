namespace BookRental.DTOs;

public class DailyRentalStatsDto
{
    public DateTime Date { get; set; }
    public int RentsCount { get; set; }
    public int ReturnsCount { get; set; }
}

