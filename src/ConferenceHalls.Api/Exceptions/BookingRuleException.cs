namespace ConferenceHalls.Api.Exceptions;

// Бросается, когда запрос нарушает правила бронирования (ответ 400)
public class BookingRuleException : Exception
{
    public BookingRuleException(string message) : base(message)
    {
    }
}