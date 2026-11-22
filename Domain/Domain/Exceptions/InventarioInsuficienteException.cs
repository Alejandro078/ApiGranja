using Domain.Enums;

namespace Domain.Exceptions;

public class InventarioInsuficienteException : Exception
{
    public InventarioInsuficienteException(TipoProducto tipo, decimal cantidad)
        : base($"No hay existencias suficientes de {tipo}. Cantidad solicitada: {cantidad}.") { }
}
