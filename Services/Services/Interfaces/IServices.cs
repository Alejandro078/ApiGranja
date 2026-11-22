using Domain.Enums;
using Services.Dtos;

namespace Services.Interfaces;

public interface IGranjaService
{
    Task<GranjaResponse> ObtenerConfiguracionAsync(CancellationToken cancellationToken = default);
    Task<GranjaResponse> ActualizarConfiguracionAsync(GranjaRequest request, CancellationToken cancellationToken = default);
}

public interface IProduccionService
{
    Task<ProduccionResponse> RegistrarProduccionDiariaAsync(ProduccionRequest request, CancellationToken cancellationToken = default);
    Task<TransformacionResponse> TransformarLecheAsync(TransformacionRequest request, CancellationToken cancellationToken = default);
}

public interface IInventarioService
{
    Task<List<LoteInventarioResponse>> ObtenerInventarioAsync(CancellationToken cancellationToken = default);
    Task<VencimientosResponse> ProcesarVencimientosAsync(DateOnly? fecha, CancellationToken cancellationToken = default);
}

public interface IVentaService
{
    Task<VentaResponse> RegistrarVentaAsync(VentaRequest request, CancellationToken cancellationToken = default);
    Task<List<VentaResponse>> ObtenerVentasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default);
}

public interface IReporteService
{
    Task<ReporteVentasResponse> ObtenerVentasAsync(DateOnly? desde, DateOnly? hasta, TipoProducto? tipo, CancellationToken cancellationToken = default);
    Task<List<LecheDiariaResponse>> ObtenerLecheDiariaAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default);
    Task<List<ProductoDiarioResponse>> ObtenerProductoDiarioAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default);
    Task<List<ProductoTotalResponse>> ObtenerProductoTotalAsync(CancellationToken cancellationToken = default);
    Task<List<ProductoDanadoResponse>> ObtenerProductoDanadoAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default);
    Task<ReporteVacasResponse> ObtenerReporteVacasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default);
}
