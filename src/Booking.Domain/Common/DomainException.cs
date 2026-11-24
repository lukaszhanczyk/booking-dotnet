namespace Booking.Domain.Common;

public class DomainException(string message) : Exception(message)
{
}