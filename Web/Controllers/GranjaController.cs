using Microsoft.AspNetCore.Mvc;
using Services.Dtos;
using Services.Interfaces;

namespace ApiSampleFinal.Controllers;

[ApiController]
[Route("api/granja")]
public class GranjaController : ControllerBase
{
    private readonly IGranjaService _service;
    public GranjaController(IGranjaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<GranjaResponse>> Obtener(CancellationToken cancellationToken) =>
        Ok(await _service.ObtenerConfiguracionAsync(cancellationToken));

    [HttpPut]
    public async Task<ActionResult<GranjaResponse>> Actualizar([FromBody] GranjaRequest request, CancellationToken cancellationToken) =>
        Ok(await _service.ActualizarConfiguracionAsync(request, cancellationToken));
}
