using ConferenceHalls.Api.DTO;

namespace ConferenceHalls.Api.Services;

public interface IHallService
{
    Task<List<HallResponseDto>> GetAllAsync();
}