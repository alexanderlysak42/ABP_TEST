using System.ComponentModel.DataAnnotations;

namespace ConferenceHalls.Api.DTO;

public class HallSearchRequestDto
{
    [Required]
    public DateOnly? Date { get; set; }

    [Required]
    public TimeOnly? StartTime { get; set; }

    [Required]
    public TimeOnly? EndTime { get; set; }

    [Range(1, 10000)]
    public int Capacity { get; set; }
}
