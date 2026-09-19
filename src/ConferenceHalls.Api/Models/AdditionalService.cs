namespace ConferenceHalls.Api.Models;

public class AdditionalService
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public Hall Hall { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}