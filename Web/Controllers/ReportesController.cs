using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Services.Dtos;
using Services.Interfaces;

namespace ApiSampleFinal.Controllers;

[ApiController]
[Route("api/reportes")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _service;
    public ReportesController(IReporteService service) => _service = service;

    [HttpGet("ventas")]
    public async Task<ActionResult<ReporteVentasResponse>> Ventas([FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, [FromQuery] TipoProducto? tipo, CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerVentasAsync(desde, hasta, tipo, cancellationToken));

    [HttpGet("leche-diaria")]
    public async Task<ActionResult<List<LecheDiariaResponse>>> LecheDiaria([FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerLecheDiariaAsync(desde, hasta, cancellationToken));

    [HttpGet("producto-diario")]
    public async Task<ActionResult<List<ProductoDiarioResponse>>> ProductoDiario([FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerProductoDiarioAsync(desde, hasta, cancellationToken));

    [HttpGet("producto-total")]
    public async Task<ActionResult<List<ProductoTotalResponse>>> ProductoTotal(CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerProductoTotalAsync(cancellationToken));

    [HttpGet("producto-danado")]
    public async Task<ActionResult<List<ProductoDanadoResponse>>> ProductoDanado([FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerProductoDanadoAsync(desde, hasta, cancellationToken));

    [HttpGet("vacas")]
    public async Task<ActionResult<ReporteVacasResponse>> Vacas([FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerReporteVacasAsync(desde, hasta, cancellationToken));
}
