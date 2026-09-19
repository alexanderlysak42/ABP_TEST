using System.ComponentModel.DataAnnotations;

namespace ConferenceHalls.Api.DTO;

public class ReportPeriodRequestDto
{
    [Required]
    public DateOnly? From { get; set; }

    [Required]
    public DateOnly? To { get; set; }
}
