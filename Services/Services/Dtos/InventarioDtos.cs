using Domain.Enums;

namespace Services.Dtos;

public class LoteInventarioResponse
{
    public int Id { get; set; }
    public TipoProducto TipoProducto { get; set; }
    public DateOnly FechaProduccion { get; set; }
    public DateOnly FechaVencimiento { get; set; }
    public decimal CantidadInicial { get; set; }
    public decimal CantidadDisponible { get; set; }
    public decimal CantidadDanada { get; set; }
}

public class VencimientosResponse
{
    public DateOnly FechaProceso { get; set; }
    public decimal CantidadDanada { get; set; }
    public int LotesProcesados { get; set; }
}
