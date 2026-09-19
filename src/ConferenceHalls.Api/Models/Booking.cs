namespace ConferenceHalls.Api.Models;

public class Booking
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public Hall Hall { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal HallCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalCost { get; set; }
    public List<AdditionalService> AdditionalServices { get; set; } = [];

}