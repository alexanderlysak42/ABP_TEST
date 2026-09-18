namespace ConferenceHalls.Api.Models;

public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourlyPrice { get; set; }

}