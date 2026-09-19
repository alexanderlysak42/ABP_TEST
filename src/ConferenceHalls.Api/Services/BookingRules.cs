namespace ConferenceHalls.Api.Services;

// Общие правила рабочего времени для бронирования
public static class BookingRules
{
    // Час открытия: бронь не может начаться раньше
    public const int OpeningHour = 6;
    // Час закрытия: бронь не может закончиться позже
    public const int ClosingHour = 23;
    // Сколько часов зал работает за сутки (нужно для отчёта о загрузке)
    public const int WorkingHoursPerDay = ClosingHour - OpeningHour;
}