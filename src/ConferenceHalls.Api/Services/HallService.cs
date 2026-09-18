using ConferenceHalls.Api.Data;
using ConferenceHalls.Api.DTO;
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
}