namespace ConferenceHalls.Api.Services;

public static class BookingRules
{
    public const int OpeningHour = 6;
    public const int ClosingHour = 23;
    public const int WorkingHoursPerDay = ClosingHour - OpeningHour;
}