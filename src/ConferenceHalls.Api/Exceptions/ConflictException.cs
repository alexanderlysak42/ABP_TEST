namespace ConferenceHalls.Api.Exceptions;

// Бросается при конфликте данных: дубликат имени или занятое время (ответ 409)
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
