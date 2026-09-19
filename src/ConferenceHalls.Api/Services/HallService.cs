using ConferenceHalls.Api.Data;
using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Exceptions;
using ConferenceHalls.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceHalls.Api.Services;

public class HallService : IHallService
{
    private readonly AppDbContext _dbContext;
    
    public HallService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<HallResponseDto>> GetAllAsync()
    {
        var halls = await _dbContext.Halls.AsNoTracking().OrderBy(h => h.Id).ToListAsync();
        return halls.Select(HallResponseDto.FromEntity).ToList();
    }

    public async Task<HallResponseDto> GetByIdAsync(int id)
    {
        var hall = await _dbContext.Halls.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
        if (hall is null)
        {
            throw new NotFoundException($"Hall with id {id} not found");
        }
        
        return HallResponseDto.FromEntity(hall);
    }

    public async Task<HallResponseDto>CreateAsync(CreateHallDto createHallDto)
    {
        if (await _dbContext.Halls.AnyAsync(h => h.Name == createHallDto.Name))
        {
            throw new ConflictException($"Hall with name '{createHallDto.Name}' already exists");
        }
        
        var hall = new Hall
        {
            Name = createHallDto.Name,
            Capacity = createHallDto.Capacity,
            BaseHourlyPrice = createHallDto.BaseHourlyPrice
        };
        
        _dbContext.Halls.Add(hall);
        await _dbContext.SaveChangesAsync();
        
        return HallResponseDto.FromEntity(hall);
    }

    public async Task<HallResponseDto> UpdateAsync(int id, UpdateHallDto updateHallDto)
    {
        if (await _dbContext.Halls.AnyAsync(h => h.Name == updateHallDto.Name && h.Id != id))
        {
            throw new ConflictException($"Hall with name '{updateHallDto.Name}' already exists");
        }
        
        var hall = await _dbContext.Halls.FirstOrDefaultAsync(h => h.Id == id);
        if (hall is null)
        {
            throw new NotFoundException($"Hall with id {id} not found");
        }
        
        hall.Name = updateHallDto.Name;
        hall.Capacity = updateHallDto.Capacity;
        hall.BaseHourlyPrice = updateHallDto.BaseHourlyPrice;
        
        await _dbContext.SaveChangesAsync();
        return HallResponseDto.FromEntity(hall);
    }

    public async Task DeleteAsync(int id)
    {
        var hall = await _dbContext.Halls.FirstOrDefaultAsync(h => h.Id == id);
        if (hall is null)
        {
            throw new NotFoundException($"Hall with id {id} not found");
        }

        if (await _dbContext.Bookings.AnyAsync(b => b.HallId == id))
        {
            throw new ConflictException($"Hall with id {id} has bookings and cannot be deleted");
        }

        _dbContext.Halls.Remove(hall);
        await _dbContext.SaveChangesAsync();
    }
}