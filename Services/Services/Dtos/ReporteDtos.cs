using Domain.Enums;

namespace Services.Dtos;

public class ReporteVentasResponse
{
    public List<VentaResponse> Ventas { get; set; } = new();
    public decimal TotalGeneral { get; set; }
}

public class LecheDiariaResponse
{
    public DateOnly Fecha { get; set; }
    public decimal LitrosProducidos { get; set; }
    public decimal LitrosTransformados { get; set; }
    public decimal LitrosNoAprovechados { get; set; }
}

public class ProductoDiarioResponse
{
    public DateOnly Fecha { get; set; }
    public TipoProducto TipoProducto { get; set; }
    public decimal CantidadProducida { get; set; }
}

public class ProductoTotalResponse
{
    public TipoProducto TipoProducto { get; set; }
    public decimal TotalProducido { get; set; }
    public decimal TotalVendido { get; set; }
    public decimal Disponible { get; set; }
    public decimal Danado { get; set; }
}

public class ProductoDanadoResponse
{
    public int LoteId { get; set; }
    public TipoProducto TipoProducto { get; set; }
    public DateOnly FechaProduccion { get; set; }
    public DateOnly FechaVencimiento { get; set; }
    public decimal CantidadDanada { get; set; }
}

public class VacaProduccionResponse
{
    public int VacaId { get; set; }
    public string Raza { get; set; } = string.Empty;
    public decimal TotalLitros { get; set; }
    public decimal PromedioLitrosPorDia { get; set; }
}

public class ReporteVacasResponse
{
    public decimal PromedioGeneralPorVaca { get; set; }
    public VacaProduccionResponse? VacaQueMasProduce { get; set; }
    public VacaProduccionResponse? VacaQueMenosProduce { get; set; }
    public List<VacaProduccionResponse> VacasDebajoDelPromedio { get; set; } = new();
}
