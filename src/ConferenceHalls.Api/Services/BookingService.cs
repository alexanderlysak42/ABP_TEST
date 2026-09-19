using ConferenceHalls.Api.Data;
using ConferenceHalls.Api.DTO;
using ConferenceHalls.Api.Exceptions;
using ConferenceHalls.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceHalls.Api.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _dbContext;

    public BookingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BookingResponseDto>> GetAllAsync()
    {
        var bookings = await _dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Hall)
            .Include(b => b.AdditionalServices)
            .OrderByDescending(b => b.StartTime)
            .ToListAsync();

        return bookings.Select(b => BookingResponseDto.FromEntity(b, b.Hall.Name)).ToList();
    }

    public async Task<BookingResponseDto> GetByIdAsync(int id)
    {
        var booking = await _dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Hall)
            .Include(b => b.AdditionalServices)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null)
        {
            throw new NotFoundException($"Booking with id {id} not found");
        }

        return BookingResponseDto.FromEntity(booking, booking.Hall.Name);
    }

    // Создаёт бронь: проверяет правила, считает цену и сохраняет
    public async Task<BookingResponseDto> CreateAsync(BookingRequestDto request)
    {
        // Загружаем зал вместе с его услугами, чтобы проверить выбранные услуги
        var hall = await _dbContext.Halls
            .Include(h => h.AdditionalServices)
            .FirstOrDefaultAsync(h => h.Id == request.HallId);

        if (hall is null)
        {
            throw new NotFoundException($"Hall with id {request.HallId} not found");
        }

        var date = request.Date!.Value;
        var startTime = request.StartTime!.Value;

        // Бронь начинается только с начала часа
        if (startTime.Minute != 0 || startTime.Second != 0)
        {
            throw new BookingRuleException("Booking must start at the beginning of an hour");
        }

        // Собираем дату и время начала и конца брони
        var start = date.ToDateTime(startTime);
        var end = start.AddHours(request.DurationHours);
        var closing = date.ToDateTime(new TimeOnly(BookingRules.ClosingHour, 0));

        // Бронь должна целиком попадать в рабочее время
        if (start.Hour < BookingRules.OpeningHour || end > closing)
        {
            throw new BookingRuleException(
                $"Booking must be within {BookingRules.OpeningHour}:00 - {BookingRules.ClosingHour}:00");
        }

        // Повторы в списке услуг убираем; услуги должны принадлежать этому залу
        var serviceIds = request.ServiceIds.Distinct().ToList();
        var selectedServices = hall.AdditionalServices.Where(s => serviceIds.Contains(s.Id)).ToList();
        if (selectedServices.Count != serviceIds.Count)
        {
            throw new BookingRuleException("Some services do not belong to the selected hall");
        }

        // Зал занят, если есть бронь, которая пересекается по времени (встык можно)
        var isBusy = await _dbContext.Bookings.AnyAsync(b =>
            b.HallId == hall.Id && b.StartTime < end && b.EndTime > start);
        if (isBusy)
        {
            throw new ConflictException("Hall is already booked for the selected time");
        }

        // Считаем цену зала по часам и добавляем фиксированную цену услуг
        var hallCost = PriceCalculator.CalculateHallCost(hall.BaseHourlyPrice, start, request.DurationHours);
        var servicesCost = selectedServices.Sum(s => s.Price);

        var booking = new Booking
        {
            HallId = hall.Id,
            StartTime = start,
            EndTime = end,
            HallCost = hallCost,
            ServicesCost = servicesCost,
            TotalCost = hallCost + servicesCost,
            AdditionalServices = selectedServices
        };

        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync();

        return BookingResponseDto.FromEntity(booking, hall.Name);
    }
}
