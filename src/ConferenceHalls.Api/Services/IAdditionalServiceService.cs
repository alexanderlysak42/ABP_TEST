using ConferenceHalls.Api.DTO;

namespace ConferenceHalls.Api.Services;

public interface IAdditionalServiceService
{
   Task<List<AdditionalServiceResponseDto>> GetAllAsync(int hallId);
   Task<AdditionalServiceResponseDto> GetByIdAsync(int hallId, int id);
   Task<AdditionalServiceResponseDto> CreateAsync(int hallId, AdditionalServiceRequestDto additionalServiceRequestDto);
   Task<AdditionalServiceResponseDto> UpdateAsync(int hallId, int id, AdditionalServiceRequestDto additionalServiceRequestDto);
   Task DeleteAsync(int hallId, int id);
}