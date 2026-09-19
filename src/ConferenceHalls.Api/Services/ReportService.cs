using ConferenceHalls.Api.Data;
using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ConferenceHalls.Api.Services;

public class ReportService : IReportService
{
    // Ограничение, чтобы не запрашивать слишком большие периоды
    private const int MaxPeriodDays = 366;

    private readonly AppDbContext _dbContext;

    public ReportService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Выручка по залам: отдельно за аренду зала, за услуги и всего
    public async Task<RevenueReportResponseDto> GetRevenueAsync(ReportPeriodRequestDto request)
    {
        var (from, to) = ValidatePeriod(request);
        var halls = await LoadHallsAsync();
        var bookings = await LoadBookingsAsync(from, to);

        var items = halls.Select(h =>
        {
            var hallBookings = bookings.Where(b => b.HallId == h.Id).ToList();
            return new HallRevenueItemDto
            {
                HallId = h.Id,
                HallName = h.Name,
                BookingsCount = hallBookings.Count,
                HallRevenue = hallBookings.Sum(b => b.HallCost),
                ServicesRevenue = hallBookings.Sum(b => b.ServicesCost),
                TotalRevenue = hallBookings.Sum(b => b.TotalCost)
            };
        }).ToList();

        return new RevenueReportResponseDto
        {
            From = from,
            To = to,
            TotalRevenue = items.Sum(i => i.TotalRevenue),
            Halls = items
        };
    }

    // Загрузка залов: сколько часов из доступных они были забронированы
    public async Task<UtilizationReportResponseDto> GetUtilizationAsync(ReportPeriodRequestDto request)
    {
        var (from, to) = ValidatePeriod(request);
        var halls = await LoadHallsAsync();
        var bookings = await LoadBookingsAsync(from, to);

        var days = to.DayNumber - from.DayNumber + 1;
        // Доступные часы = дни периода * рабочие часы в сутках
        var availableHours = days * BookingRules.WorkingHoursPerDay;

        var items = halls.Select(h =>
        {
            var bookedHours = bookings
                .Where(b => b.HallId == h.Id)
                .Sum(b => (b.EndTime - b.StartTime).TotalHours);

            return new HallUtilizationItemDto
            {
                HallId = h.Id,
                HallName = h.Name,
                BookedHours = bookedHours,
                AvailableHours = availableHours,
                UtilizationPercent = Math.Round(bookedHours / availableHours * 100, 2)
            };
        }).ToList();

        return new UtilizationReportResponseDto { From = from, To = to, Halls = items };
    }

    // Проверяет даты отчёта и возвращает их без пустых значений
    private static (DateOnly From, DateOnly To) ValidatePeriod(ReportPeriodRequestDto request)
    {
        var from = request.From!.Value;
        var to = request.To!.Value;

        if (to < from)
        {
            throw new BookingRuleException("'To' date must not be earlier than 'From' date");
        }

        if (to.DayNumber - from.DayNumber + 1 > MaxPeriodDays)
        {
            throw new BookingRuleException($"Period must not exceed {MaxPeriodDays} days");
        }

        return (from, to);
    }

    private async Task<List<(int Id, string Name)>> LoadHallsAsync()
    {
        var halls = await _dbContext.Halls
            .AsNoTracking()
            .OrderBy(h => h.Id)
            .Select(h => new { h.Id, h.Name })
            .ToListAsync();

        return halls.Select(h => (h.Id, h.Name)).ToList();
    }

    private async Task<List<BookingRow>> LoadBookingsAsync(DateOnly from, DateOnly to)
    {
        // В период попадают брони, которые начались от начала первого до конца последнего дня
        var periodStart = from.ToDateTime(TimeOnly.MinValue);
        var periodEnd = to.AddDays(1).ToDateTime(TimeOnly.MinValue);

        return await _dbContext.Bookings
            .AsNoTracking()
            .Where(b => b.StartTime >= periodStart && b.StartTime < periodEnd)
            .Select(b => new BookingRow(b.HallId, b.StartTime, b.EndTime, b.HallCost, b.ServicesCost, b.TotalCost))
            .ToListAsync();
    }

    private record BookingRow(
        int HallId,
        DateTime StartTime,
        DateTime EndTime,
        decimal HallCost,
        decimal ServicesCost,
        decimal TotalCost);
}
