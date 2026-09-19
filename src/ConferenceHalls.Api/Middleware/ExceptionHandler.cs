using ConferenceHalls.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceHalls.Api.Middleware;

// Превращает исключения в понятные ответы API с нужным кодом ошибки
public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Подбираем код ответа по типу исключения
        var (status, title, detail) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not found", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
            BookingRuleException => (StatusCodes.Status400BadRequest, "Booking rule violation", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred")
        };

        // Неожиданные ошибки пишем в лог, а клиенту не показываем подробности
        if (status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = title, Detail = detail },
            cancellationToken);

        return true;
    }
}