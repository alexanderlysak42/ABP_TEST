using ConferenceHalls.Api.Data;
using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Exceptions;
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
}