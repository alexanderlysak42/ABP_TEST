using Microsoft.AspNetCore.Mvc;

namespace ConferenceHalls.Api.Middleware;

// Проверяет заголовок X-Api-Key у всех запросов, кроме Swagger и /health
public class ApiKeyMiddleware
{
    public const string HeaderName = "X-Api-Key";

    private readonly RequestDelegate _next;
    private readonly string _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration["ApiKeys:Key"] ?? string.Empty;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        // Swagger и проверка здоровья доступны без ключа
        if (path.StartsWithSegments("/swagger") || path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // Нет ключа или он неверный: сразу отвечаем 401
        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey) || providedKey != _apiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = $"Missing or invalid {HeaderName} header"
            });
            return;
        }

        await _next(context);
    }
}
