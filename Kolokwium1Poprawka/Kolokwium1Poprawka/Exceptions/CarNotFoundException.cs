namespace Kolokwium1Poprawka.Exceptions;

public class CarNotFoundException : Exception
{
    public CarNotFoundException()
    {
    }

    public CarNotFoundException(string? message) : base(message)
    {
    }

    public CarNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}