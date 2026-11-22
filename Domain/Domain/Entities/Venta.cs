using Domain.Enums;

namespace Domain;

public class Venta
{
    public int Id { get; private set; }
    public DateOnly Fecha { get; private set; }
    public TipoProducto TipoProducto { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Total { get; private set; }
    public ICollection<VentaDetalle> Detalles { get; private set; } = new List<VentaDetalle>();

    private Venta() { }

    public Venta(DateOnly fecha, TipoProducto tipoProducto, decimal cantidad, decimal precioUnitario)
    {
        if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        if (precioUnitario < 0) throw new ArgumentException("El precio no puede ser negativo.");
        Fecha = fecha;
        TipoProducto = tipoProducto;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Total = cantidad * precioUnitario;
    }

    public void AgregarDetalle(int loteId, decimal cantidad)
    {
        var acumulado = Detalles.Sum(d => d.Cantidad);
        if (cantidad <= 0 || acumulado + cantidad > Cantidad)
            throw new InvalidOperationException("La cantidad del detalle no es válida.");
        Detalles.Add(new VentaDetalle(loteId, cantidad));
    }

    public void AgregarDetalle(LoteProducto lote, decimal cantidad)
    {
        var acumulado = Detalles.Sum(d => d.Cantidad);
        if (cantidad <= 0 || acumulado + cantidad > Cantidad)
            throw new InvalidOperationException("La cantidad del detalle no es válida.");
        Detalles.Add(new VentaDetalle(lote, cantidad));
    }
}
