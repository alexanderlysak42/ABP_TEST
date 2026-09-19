namespace ConferenceHalls.Api.DTO;

public class RevenueReportResponseDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<HallRevenueItemDto> Halls { get; set; } = [];
}

public class HallRevenueItemDto
{
    public int HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int BookingsCount { get; set; }
    public decimal HallRevenue { get; set; }
    public decimal ServicesRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
}
