using ConferenceHalls.Api.Models;
using ConferenceHalls.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace ConferenceHalls.Api.Data;

// Заполняет БД начальными данными при запуске; повторный запуск дублей не создаёт
public static class DbSeeder
{
    // Залы из ТЗ, для которых добавляем стандартные услуги
    private static readonly int[] SeededHallIds = [1, 2, 3];

    // Услуги и цены из ТЗ
    private static readonly (string Name, decimal Price)[] DefaultServices =
    [
        ("Проєктор", 500m),
        ("Wi-Fi", 300m),
        ("Звук", 700m)
    ];

    public static async Task SeedAsync(AppDbContext dbContext, bool addDemoBookings)
    {
        await SeedServicesAsync(dbContext);

        // Демо-брони добавляем только в режиме разработки
        if (addDemoBookings)
        {
            await SeedDemoBookingsAsync(dbContext);
        }
    }

    private static async Task SeedServicesAsync(AppDbContext dbContext)
    {
        var existingHallIds = await dbContext.Halls
            .Where(h => SeededHallIds.Contains(h.Id))
            .Select(h => h.Id)
            .ToListAsync();

        var existingServices = await dbContext.AdditionalServices
            .Where(s => existingHallIds.Contains(s.HallId))
            .Select(s => new { s.HallId, s.Name })
            .ToListAsync();

        foreach (var hallId in existingHallIds)
        {
            foreach (var (name, price) in DefaultServices)
            {
                // Такая услуга в зале уже есть, пропускаем
                if (existingServices.Any(s => s.HallId == hallId && s.Name == name))
                {
                    continue;
                }

                dbContext.AdditionalServices.Add(new AdditionalService
                {
                    HallId = hallId,
                    Name = name,
                    Price = price
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedDemoBookingsAsync(AppDbContext dbContext)
    {
        // Если брони уже есть, ничего не добавляем
        if (await dbContext.Bookings.AnyAsync())
        {
            return;
        }

        var halls = await dbContext.Halls
            .Include(h => h.AdditionalServices)
            .Where(h => SeededHallIds.Contains(h.Id))
            .ToDictionaryAsync(h => h.Id);

        // Демо-брони ставим на завтра, чтобы они всегда были в будущем
        var day = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

        var demo = new (int HallId, int StartHour, int Hours, string[] Services)[]
        {
            (1, 10, 4, ["Проєктор", "Wi-Fi"]),
            (2, 9, 3, ["Звук"]),
            (3, 18, 4, [])
        };

        foreach (var (hallId, startHour, hours, serviceNames) in demo)
        {
            if (!halls.TryGetValue(hallId, out var hall))
            {
                continue;
            }

            var start = day.ToDateTime(new TimeOnly(startHour, 0));
            var services = hall.AdditionalServices.Where(s => serviceNames.Contains(s.Name)).ToList();
            var hallCost = PriceCalculator.CalculateHallCost(hall.BaseHourlyPrice, start, hours);
            var servicesCost = services.Sum(s => s.Price);

            dbContext.Bookings.Add(new Booking
            {
                HallId = hall.Id,
                StartTime = start,
                EndTime = start.AddHours(hours),
                HallCost = hallCost,
                ServicesCost = servicesCost,
                TotalCost = hallCost + servicesCost,
                AdditionalServices = services
            });
        }

        await dbContext.SaveChangesAsync();
    }
}
