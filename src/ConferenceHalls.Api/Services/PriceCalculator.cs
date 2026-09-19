namespace ConferenceHalls.Api.Services;

// Считает стоимость аренды зала с учётом времени суток
public static class PriceCalculator
{
    public static decimal CalculateHallCost(decimal baseHourlyPrice, DateTime start, int durationHours)
    {
        // Каждый час брони считаем отдельно, потому что у часов разные коэффициенты
        var total = 0m;
        for (var i = 0; i < durationHours; i++)
        {
            var hour = start.AddHours(i).Hour;
            total += baseHourlyPrice * GetMultiplier(hour);
        }

        return Math.Round(total, 2);
    }

    // Возвращает коэффициент цены для часа: утро -10%, обед +15%, вечер -20%
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