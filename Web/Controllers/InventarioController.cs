using Microsoft.AspNetCore.Mvc;
using Services.Dtos;
using Services.Interfaces;

namespace ApiSampleFinal.Controllers;

[ApiController]
[Route("api/inventario")]
public class InventarioController : ControllerBase
{
    private readonly IInventarioService _service;
    public InventarioController(IInventarioService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<LoteInventarioResponse>>> Obtener(CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerInventarioAsync(cancellationToken));

    [HttpPost("procesar-vencimientos")]
    public async Task<ActionResult<VencimientosResponse>> ProcesarVencimientos([FromQuery] DateOnly? fecha, CancellationToken cancellationToken) =>
        Ok(await _service.ProcesarVencimientosAsync(fecha, cancellationToken));
}
