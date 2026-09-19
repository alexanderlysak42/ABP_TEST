using System.ComponentModel.DataAnnotations;

namespace ConferenceHalls.Api.DTO;

public class AdditionalServiceRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1000000)]
    public decimal Price { get; set; }
}
