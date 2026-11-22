namespace Domain.Exceptions;

public class ConflictoOperacionException : Exception
{
    public ConflictoOperacionException(string message) : base(message) { }
}
