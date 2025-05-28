namespace Kolokwium1Poprawka.Exceptions;

public class ClientNotFoundException : Exception
{
    public ClientNotFoundException()
    {
    }

    public ClientNotFoundException(string? message) : base(message)
    {
    }

    public ClientNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}