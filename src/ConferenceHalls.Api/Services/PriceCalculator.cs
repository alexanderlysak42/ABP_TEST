namespace ConferenceHalls.Api.Services;

public static class PriceCalculator
{
    public static decimal CalculateHallCost(decimal baseHourlyPrice, DateTime start, int durationHours)
    {
        var total = 0m;
        for (var i = 0; i < durationHours; i++)
        {
            var hour = start.AddHours(i).Hour;
            total += baseHourlyPrice * GetMultiplier(hour);
        }

        return Math.Round(total, 2);
    }

    private static decimal GetMultiplier(int hour)
    {
        if (hour >= 6 && hour < 9)
        {
            return 0.90m;
        }

        if (hour >= 12 && hour < 14)
        {
            return 1.15m;
        }

        if (hour >= 18 && hour < 23)
        {
            return 0.80m;
        }

        return 1.00m;
    }
}