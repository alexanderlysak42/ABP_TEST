namespace ConferenceHalls.Api.Exceptions;

// Бросается, когда нужная запись не найдена (ответ 404)
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}