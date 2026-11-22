namespace Domain.Exceptions;

public class LecheInsuficienteException : Exception
{
    public LecheInsuficienteException(decimal solicitada, decimal disponible)
        : base($"No hay suficiente leche. Solicitada: {solicitada}; disponible: {disponible}.") { }
}
