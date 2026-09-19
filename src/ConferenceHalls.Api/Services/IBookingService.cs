using ConferenceHalls.Api.DTO;

namespace ConferenceHalls.Api.Services;

public interface IBookingService
{
    Task<List<BookingResponseDto>> GetAllAsync();
    Task<BookingResponseDto> GetByIdAsync(int id);
    Task<BookingResponseDto> CreateAsync(BookingRequestDto request);
}