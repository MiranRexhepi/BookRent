namespace BookRental.DTOs;

public class DashboardStatsDto
{
    public int ActiveUsersThisMonth { get; set; }
    public int RentedBooksAtMoment { get; set; }
    public int OverdueBooks { get; set; }
    public int PassiveUsers { get; set; }
    public int TotalBooksAddedThisMonth { get; set; }
    public int TotalBooksAvailable { get; set; }
}

