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

    // Создаёт зал вместе со списком его услуг
    public async Task<HallResponseDto>CreateAsync(CreateHallDto createHallDto)
    {
        if (await _dbContext.Halls.AnyAsync(h => h.Name == createHallDto.Name))
        {
            throw new ConflictException($"Hall with name '{createHallDto.Name}' already exists");
        }
        
        // В одном зале названия услуг не должны повторяться
        var duplicateServiceName = createHallDto.Services
            .GroupBy(s => s.Name)
            .FirstOrDefault(g => g.Count() > 1)?.Key;
        if (duplicateServiceName is not null)
        {
            throw new ConflictException($"Service name '{duplicateServiceName}' is used more than once");
        }

        var hall = new Hall
        {
            Name = createHallDto.Name,
            Capacity = createHallDto.Capacity,
            BaseHourlyPrice = createHallDto.BaseHourlyPrice,
            AdditionalServices = createHallDto.Services
                .Select(s => new AdditionalService { Name = s.Name, Price = s.Price })
                .ToList()
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

    // Ищет залы, где хватает мест и нет броней на выбранное время
    public async Task<List<HallResponseDto>> SearchAvailableAsync(HallSearchRequestDto request)
    {
        var date = request.Date!.Value;
        var startTime = request.StartTime!.Value;
        var endTime = request.EndTime!.Value;

        if (endTime <= startTime)
        {
            throw new BookingRuleException("EndTime must be later than StartTime");
        }

        if (startTime.Hour < BookingRules.OpeningHour || endTime > new TimeOnly(BookingRules.ClosingHour, 0))
        {
            throw new BookingRuleException(
                $"Halls are available from {BookingRules.OpeningHour}:00 to {BookingRules.ClosingHour}:00");
        }

        var start = date.ToDateTime(startTime);
        var end = date.ToDateTime(endTime);

        // Сначала самые маленькие подходящие залы
        var halls = await _dbContext.Halls
            .AsNoTracking()
            .Where(h => h.Capacity >= request.Capacity)
            .Where(h => !h.Bookings.Any(b => b.StartTime < end && b.EndTime > start))
            .OrderBy(h => h.Capacity)
            .ToListAsync();

        return halls.Select(HallResponseDto.FromEntity).ToList();
    }

    public async Task DeleteAsync(int id)
    {
        var hall = await _dbContext.Halls.FirstOrDefaultAsync(h => h.Id == id);
        if (hall is null)
        {
            throw new NotFoundException($"Hall with id {id} not found");
        }

        // Зал с бронями удалять нельзя, чтобы не потерять историю
        if (await _dbContext.Bookings.AnyAsync(b => b.HallId == id))
        {
            throw new ConflictException($"Hall with id {id} has bookings and cannot be deleted");
        }

        _dbContext.Halls.Remove(hall);
        await _dbContext.SaveChangesAsync();
    }
}