using ConferenceHalls.Api.Models;

namespace ConferenceHalls.Api.DTO;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationHours { get; set; }
    public decimal HallCost { get; set; }
    public decimal ServicesCost { get; set; }
    public decimal TotalCost { get; set; }
    public List<AdditionalServiceResponseDto> Services { get; set; } = [];

    public static BookingResponseDto FromEntity(Booking booking, string hallName)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            HallId = booking.HallId,
            HallName = hallName,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            DurationHours = (int)(booking.EndTime - booking.StartTime).TotalHours,
            HallCost = booking.HallCost,
            ServicesCost = booking.ServicesCost,
            TotalCost = booking.TotalCost,
            Services = booking.AdditionalServices
                .OrderBy(s => s.Id)
                .Select(AdditionalServiceResponseDto.FromEntity)
                .ToList()
        };
    }

}