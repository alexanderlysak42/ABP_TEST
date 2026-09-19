namespace ConferenceHalls.Api.Exceptions;

public class BookingRuleException : Exception
{
    public BookingRuleException(string message) : base(message)
    {
    }
}