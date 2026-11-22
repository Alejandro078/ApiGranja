using Microsoft.AspNetCore.Mvc;
using Services.Dtos;
using Services.Interfaces;

namespace ApiSampleFinal.Controllers;

[ApiController]
[Route("api/ventas")]
public class VentasController : ControllerBase
{
    private readonly IVentaService _service;
    public VentasController(IVentaService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<VentaResponse>> Registrar([FromBody] VentaRequest request, CancellationToken cancellationToken)
    {
        var response = await _service.RegistrarVentaAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<VentaResponse>>> Obtener([FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerVentasAsync(desde, hasta, cancellationToken));
}
