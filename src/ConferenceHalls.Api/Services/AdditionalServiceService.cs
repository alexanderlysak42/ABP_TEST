using ConferenceHalls.Api.Data;
using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Exceptions;
using ConferenceHalls.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceHalls.Api.Services;

public class AdditionalServiceService : IAdditionalServiceService
{
    private readonly AppDbContext _dbContext;

    public AdditionalServiceService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<AdditionalServiceResponseDto>> GetAllAsync(int hallId)
    {
        await HallExistsAsync(hallId);
        
        var additionalServices = await _dbContext.AdditionalServices
            .AsNoTracking()
            .Where(a => a.HallId == hallId)
            .OrderBy(s => s.Id)
            .ToListAsync();

        return additionalServices.Select(AdditionalServiceResponseDto.FromEntity).ToList();
    }

    public async Task<AdditionalServiceResponseDto> GetByIdAsync(int hallId, int id)
    {
        await HallExistsAsync(hallId);
        
        var additionalService = await  _dbContext.AdditionalServices
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.HallId == hallId);

        if (additionalService is null)
        {
            throw new NotFoundException($"Additional service with id {id} not found in hall {hallId}");
        }
        
        return AdditionalServiceResponseDto.FromEntity(additionalService);
    }

    public async Task<AdditionalServiceResponseDto> CreateAsync(int hallId, AdditionalServiceRequestDto additionalServiceRequestDto)
    {
        await HallExistsAsync(hallId);

        if (await _dbContext.AdditionalServices.AnyAsync(s =>
                s.HallId == hallId && s.Name == additionalServiceRequestDto.Name))
        {
            throw new ConflictException($"Additional service with name '{additionalServiceRequestDto.Name}' already exists in hall {hallId}");
        }

        var additionalService = new AdditionalService
        {
            HallId = hallId,
            Name = additionalServiceRequestDto.Name,
            Price = additionalServiceRequestDto.Price
        };
        
        _dbContext.AdditionalServices.Add(additionalService);
        await _dbContext.SaveChangesAsync();
        return AdditionalServiceResponseDto.FromEntity(additionalService);
    }

    public async Task<AdditionalServiceResponseDto> UpdateAsync(int hallId, int id, AdditionalServiceRequestDto additionalServiceRequestDto)
    {
        await HallExistsAsync(hallId);
        
        var additionalService = await _dbContext.AdditionalServices.FirstOrDefaultAsync(s => s.Id == id && s.HallId == hallId);
        if (additionalService is null)
        {
            throw new NotFoundException($"Additional service with id {id} not found in hall {hallId}");
        }
        
        if (await _dbContext.AdditionalServices.AnyAsync(s => s.HallId == hallId && s.Name == additionalServiceRequestDto.Name && s.Id != id))
        {
            throw new ConflictException($"Additional service with name '{additionalServiceRequestDto.Name}' already exists in hall {hallId}");
        }

        additionalService.Name = additionalServiceRequestDto.Name;
        additionalService.Price = additionalServiceRequestDto.Price;

        await _dbContext.SaveChangesAsync();
  
        return AdditionalServiceResponseDto.FromEntity(additionalService);

    }

    public async Task DeleteAsync(int hallId, int id)
    {
        await HallExistsAsync(hallId);

        var service = await _dbContext.AdditionalServices.FirstOrDefaultAsync(s => s.Id == id && s.HallId == hallId);
        if (service is null)
        {
            throw new NotFoundException($"Service with id {id} not found in hall {hallId}");
        }

        _dbContext.AdditionalServices.Remove(service);
        await _dbContext.SaveChangesAsync();

    }

    // Проверяет, что зал существует, иначе бросает 404
    private async Task HallExistsAsync(int hallId)
    {
        if (!await _dbContext.Halls.AnyAsync(h => h.Id == hallId))
        {
            throw new NotFoundException($"Hall with id {hallId} not found");
        }
    }

}