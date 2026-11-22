namespace Domain;

public class VentaDetalle
{
    public int Id { get; private set; }
    public int VentaId { get; private set; }
    public int LoteId { get; private set; }
    public decimal Cantidad { get; private set; }
    public Venta? Venta { get; private set; }
    public LoteProducto? Lote { get; private set; }

    private VentaDetalle() { }

    public VentaDetalle(int loteId, decimal cantidad)
    {
        if (loteId <= 0) throw new ArgumentException("Lote inválido.");
        if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        LoteId = loteId;
        Cantidad = cantidad;
    }

    public VentaDetalle(LoteProducto lote, decimal cantidad)
    {
        if (lote is null) throw new ArgumentException("Lote inválido.");
        if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        Lote = lote;
        LoteId = lote.Id;
        Cantidad = cantidad;
    }
}
