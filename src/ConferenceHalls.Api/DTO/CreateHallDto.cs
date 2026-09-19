using System.ComponentModel.DataAnnotations;

namespace ConferenceHalls.Api.DTO;

public class CreateHallDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } =  string.Empty;
    
    [Range(1, 1000)]
    public int Capacity { get; set; }
    
    [Range(0.01, 1000000)]
    public decimal BaseHourlyPrice { get; set; }

    public List<AdditionalServiceRequestDto> Services { get; set; } = [];
}