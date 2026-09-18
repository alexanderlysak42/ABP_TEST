using ConferenceHalls.Api.Models;

namespace ConferenceHalls.Api.DTO;

public class HallResponseDto
{
    public int  Id { get; set; }
    public string Name { get; set; } =  string.Empty;
    public int Capacity { get; set; }
    public decimal BaseHourlyPrice { get; set; }

    public static HallResponseDto FromEntity(Hall hall)
    {
        return new HallResponseDto
        {
            Id = hall.Id,
            Name = hall.Name,
            Capacity = hall.Capacity,
            BaseHourlyPrice = hall.BaseHourlyPrice
        };

    }
}