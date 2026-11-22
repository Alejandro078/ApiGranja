using Domain.Enums;

namespace Services.Dtos;

public class VentaRequest
{
    public TipoProducto TipoProducto { get; set; }
    public decimal Cantidad { get; set; }
    public DateOnly? Fecha { get; set; }
}

public class VentaDetalleResponse
{
    public int LoteId { get; set; }
    public DateOnly FechaProduccion { get; set; }
    public decimal Cantidad { get; set; }
}

public class VentaResponse
{
    public int Id { get; set; }
    public DateOnly Fecha { get; set; }
    public TipoProducto TipoProducto { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total { get; set; }
    public List<VentaDetalleResponse> Detalle { get; set; } = new();
}
