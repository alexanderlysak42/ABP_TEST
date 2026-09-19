using ConferenceHalls.Api.DTO;

namespace ConferenceHalls.Api.Services;

public interface IHallService
{
    Task<List<HallResponseDto>> GetAllAsync();
    
    Task<HallResponseDto> GetByIdAsync(int id);
    
    Task<HallResponseDto> CreateAsync(CreateHallDto createHallDto);
    
    Task<HallResponseDto> UpdateAsync(int id, UpdateHallDto updateHallDto);
    
    Task DeleteAsync(int id);
}