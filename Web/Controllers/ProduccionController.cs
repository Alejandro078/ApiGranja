using Microsoft.AspNetCore.Mvc;
using Services.Dtos;
using Services.Interfaces;

namespace ApiSampleFinal.Controllers;

[ApiController]
[Route("api/produccion")]
public class ProduccionController : ControllerBase
{
    private readonly IProduccionService _service;
    public ProduccionController(IProduccionService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<ProduccionResponse>> Registrar([FromBody] ProduccionRequest request, CancellationToken cancellationToken) =>
        Ok(await _service.RegistrarProduccionDiariaAsync(request, cancellationToken));

    [HttpPost("transformar")]
    public async Task<ActionResult<TransformacionResponse>> Transformar([FromBody] TransformacionRequest request, CancellationToken cancellationToken) =>
        Ok(await _service.TransformarLecheAsync(request, cancellationToken));
}
