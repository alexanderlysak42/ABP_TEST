using ConferenceHalls.Api.Models;

namespace ConferenceHalls.Api.DTO;

public class AdditionalServiceResponseDto
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    public static AdditionalServiceResponseDto FromEntity(AdditionalService service)
    {
        return new AdditionalServiceResponseDto
        {
            Id = service.Id,
            HallId = service.HallId,
            Name = service.Name,
            Price = service.Price
        };
    }
}