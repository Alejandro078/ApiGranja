using AutoMapper;
using Domain.Interfaces;
using Services.Dtos;
using Services.Interfaces;

namespace Services.Services;

public class InventarioService : IInventarioService
{
    private readonly ILoteRepository _lotes;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IMapper _mapper;

    public InventarioService(ILoteRepository lotes, IUnitOfWork uow, IClock clock, IMapper mapper)
    {
        _lotes = lotes;
        _uow = uow;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<List<LoteInventarioResponse>> ObtenerInventarioAsync(CancellationToken cancellationToken = default)
    {
        var lotes = await _lotes.ObtenerTodosAsync(cancellationToken);
        return _mapper.Map<List<LoteInventarioResponse>>(lotes);
    }

    public async Task<VencimientosResponse> ProcesarVencimientosAsync(DateOnly? fecha, CancellationToken cancellationToken = default)
    {
        var fechaProceso = fecha ?? _clock.Today;
        var lotes = await _lotes.ObtenerTodosAsync(cancellationToken);
        decimal danado = 0;
        var procesados = 0;
        foreach (var lote in lotes)
        {
            if (lote.EstaVencido(fechaProceso) && lote.CantidadDisponible > 0)
            {
                danado += lote.MarcarComoDanado();
                procesados++;
            }
        }
        if (procesados > 0) await _uow.SaveChangesAsync(cancellationToken);
        return new VencimientosResponse { FechaProceso = fechaProceso, CantidadDanada = danado, LotesProcesados = procesados };
    }
}
