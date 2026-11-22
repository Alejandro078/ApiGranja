using AutoMapper;
using Domain;
using Domain.Exceptions;
using Domain.Interfaces;
using Services.Dtos;
using Services.Interfaces;

namespace Services.Services;

public class GranjaService : IGranjaService
{
    private readonly IGranjaRepository _granjas;
    private readonly ICowRepository _vacas;
    private readonly IProduccionLecheRepository _producciones;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GranjaService(IGranjaRepository granjas, ICowRepository vacas,
        IProduccionLecheRepository producciones, IUnitOfWork uow, IMapper mapper)
    {
        _granjas = granjas;
        _vacas = vacas;
        _producciones = producciones;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<GranjaResponse> ObtenerConfiguracionAsync(CancellationToken cancellationToken = default)
    {
        var granja = await _granjas.ObtenerAsync(cancellationToken);
        if (granja is null)
        {
            // Valores sugeridos en el taller. El precio del yogurt queda en cero hasta configurarlo.
            granja = new Granja(10, 15m, 10m, 1m, 30, 15, 28000m, 0m);
            await _granjas.AgregarAsync(granja, cancellationToken);
        }

        await AsegurarVacasConfiguradasAsync(granja.NumeroVacas, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        var response = _mapper.Map<GranjaResponse>(granja);
        response.ProduccionDiariaTotal = granja.CalcularProduccionDiaria();
        return response;
    }

    public async Task<GranjaResponse> ActualizarConfiguracionAsync(GranjaRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentException("Debe enviar la configuración de la granja.");

        var granja = await _granjas.ObtenerAsync(cancellationToken);
        if (granja is null)
        {
            granja = new Granja(request.NumeroVacas, request.LitrosPorVacaDia,
                request.RendimientoQueso, request.RendimientoYogurt,
                request.VidaUtilQueso, request.VidaUtilYogurt,
                request.PrecioQueso, request.PrecioYogurt);
            await _granjas.AgregarAsync(granja, cancellationToken);
        }
        else
        {
            granja.ActualizarConfiguracion(request.NumeroVacas, request.LitrosPorVacaDia,
                request.RendimientoQueso, request.RendimientoYogurt,
                request.VidaUtilQueso, request.VidaUtilYogurt,
                request.PrecioQueso, request.PrecioYogurt);
        }

        await AsegurarVacasConfiguradasAsync(granja.NumeroVacas, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        var response = _mapper.Map<GranjaResponse>(granja);
        response.ProduccionDiariaTotal = granja.CalcularProduccionDiaria();
        return response;
    }

    private async Task AsegurarVacasConfiguradasAsync(int numeroVacas, CancellationToken cancellationToken)
    {
        var existentes = await _vacas.ObtenerTodosAsync(cancellationToken);
        if (existentes.Count > numeroVacas)
        {
            var sobrantes = existentes.OrderByDescending(v => v.Id).Take(existentes.Count - numeroVacas).ToList();
            foreach (var vaca in sobrantes)
            {
                if (await _producciones.TieneProduccionAsync(vaca.Id, cancellationToken))
                    throw new ConflictoOperacionException("No se puede reducir el número de vacas porque hay vacas con producción registrada.");
                await _vacas.EliminarAsync(vaca, cancellationToken);
            }
        }

        for (var i = existentes.Count; i < numeroVacas; i++)
            await _vacas.AgregarAsync(new Cow { Race = "No especificada" }, cancellationToken);
    }
}
