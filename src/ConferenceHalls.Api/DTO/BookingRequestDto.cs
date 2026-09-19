using System.ComponentModel.DataAnnotations;

namespace ConferenceHalls.Api.DTO;

public class BookingRequestDto
{
    [Range(1, int.MaxValue)]
    public int HallId { get; set; }

    [Required]
    public DateOnly? Date { get; set; }

    [Required]
    public TimeOnly? StartTime { get; set; }

    [Range(1, 17)]
    public int DurationHours { get; set; }

    public List<int> ServiceIds { get; set; } = [];
}
