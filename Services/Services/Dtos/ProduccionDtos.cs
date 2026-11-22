using Domain.Enums;

namespace Services.Dtos;

public class ProduccionRequest
{
    public DateOnly? Fecha { get; set; }
}

public class ProduccionResponse
{
    public DateOnly Fecha { get; set; }
    public int NumeroVacas { get; set; }
    public decimal LitrosPorVaca { get; set; }
    public decimal LitrosTotales { get; set; }
}

public class TransformacionRequest
{
    public DateOnly? Fecha { get; set; }
    public TipoProducto TipoProducto { get; set; }
    public decimal Litros { get; set; }
}

public class TransformacionResponse
{
    public int LoteId { get; set; }
    public DateOnly FechaProduccion { get; set; }
    public DateOnly FechaVencimiento { get; set; }
    public TipoProducto TipoProducto { get; set; }
    public decimal LitrosUsados { get; set; }
    public decimal CantidadObtenida { get; set; }
}
