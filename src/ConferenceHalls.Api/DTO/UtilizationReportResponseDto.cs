namespace ConferenceHalls.Api.DTO;

public class UtilizationReportResponseDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public List<HallUtilizationItemDto> Halls { get; set; } = [];
}

public class HallUtilizationItemDto
{
    public int HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public double BookedHours { get; set; }
    public int AvailableHours { get; set; }
    public double UtilizationPercent { get; set; }
}
