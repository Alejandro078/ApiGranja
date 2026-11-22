using Domain.Enums;
using Domain.Interfaces;
using Services.Dtos;
using Services.Interfaces;

namespace Services.Services;

public class ReporteService : IReporteService
{
    private readonly IVentaRepository _ventas;
    private readonly IProduccionLecheRepository _producciones;
    private readonly ILoteRepository _lotes;
    private readonly ICowRepository _vacas;

    public ReporteService(IVentaRepository ventas, IProduccionLecheRepository producciones,
        ILoteRepository lotes, ICowRepository vacas)
    {
        _ventas = ventas;
        _producciones = producciones;
        _lotes = lotes;
        _vacas = vacas;
    }

    public async Task<ReporteVentasResponse> ObtenerVentasAsync(DateOnly? desde, DateOnly? hasta, TipoProducto? tipo, CancellationToken cancellationToken = default)
    {
        var ventas = await _ventas.ObtenerEntreFechasAsync(desde, hasta, cancellationToken);
        var filtradas = ventas.Where(v => !tipo.HasValue || v.TipoProducto == tipo.Value).ToList();
        var resultado = filtradas.Select(VentaService.MapearVenta).ToList();
        return new ReporteVentasResponse { Ventas = resultado, TotalGeneral = resultado.Sum(v => v.Total) };
    }

    public async Task<List<LecheDiariaResponse>> ObtenerLecheDiariaAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default)
    {
        var registros = await _producciones.ObtenerEntreFechasAsync(desde, hasta, cancellationToken);
        return registros.GroupBy(p => p.Fecha).OrderBy(g => g.Key).Select(g => new LecheDiariaResponse
        {
            Fecha = g.Key,
            LitrosProducidos = g.Sum(p => p.LitrosProducidos),
            LitrosTransformados = g.Sum(p => p.LitrosTransformados),
            LitrosNoAprovechados = g.Sum(p => p.LitrosNoAprovechados)
        }).ToList();
    }

    public async Task<List<ProductoDiarioResponse>> ObtenerProductoDiarioAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default)
    {
        var lotes = await _lotes.ObtenerTodosAsync(cancellationToken);
        return lotes.Where(l => (!desde.HasValue || l.FechaProduccion >= desde.Value) && (!hasta.HasValue || l.FechaProduccion <= hasta.Value))
            .GroupBy(l => new { l.FechaProduccion, l.Tipo })
            .OrderBy(g => g.Key.FechaProduccion).ThenBy(g => g.Key.Tipo)
            .Select(g => new ProductoDiarioResponse { Fecha = g.Key.FechaProduccion, TipoProducto = g.Key.Tipo, CantidadProducida = g.Sum(l => l.CantidadInicial) })
            .ToList();
    }

    public async Task<List<ProductoTotalResponse>> ObtenerProductoTotalAsync(CancellationToken cancellationToken = default)
    {
        var lotes = await _lotes.ObtenerTodosAsync(cancellationToken);
        var ventas = await _ventas.ObtenerEntreFechasAsync(null, null, cancellationToken);
        return Enum.GetValues<TipoProducto>().Select(tipo => new ProductoTotalResponse
        {
            TipoProducto = tipo,
            TotalProducido = lotes.Where(l => l.Tipo == tipo).Sum(l => l.CantidadInicial),
            TotalVendido = ventas.Where(v => v.TipoProducto == tipo).Sum(v => v.Cantidad),
            Disponible = lotes.Where(l => l.Tipo == tipo).Sum(l => l.CantidadDisponible),
            Danado = lotes.Where(l => l.Tipo == tipo).Sum(l => l.CantidadDanada)
        }).ToList();
    }

    public async Task<List<ProductoDanadoResponse>> ObtenerProductoDanadoAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default)
    {
        var lotes = await _lotes.ObtenerTodosAsync(cancellationToken);
        return lotes.Where(l => l.CantidadDanada > 0 && (!desde.HasValue || l.FechaVencimiento >= desde.Value) && (!hasta.HasValue || l.FechaVencimiento <= hasta.Value))
            .Select(l => new ProductoDanadoResponse
            {
                LoteId = l.Id,
                TipoProducto = l.Tipo,
                FechaProduccion = l.FechaProduccion,
                FechaVencimiento = l.FechaVencimiento,
                CantidadDanada = l.CantidadDanada
            }).ToList();
    }

    public async Task<ReporteVacasResponse> ObtenerReporteVacasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default)
    {
        var vacas = await _vacas.ObtenerTodosAsync(cancellationToken);
        var producciones = await _producciones.ObtenerEntreFechasAsync(desde, hasta, cancellationToken);
        var porVaca = vacas.Select(v =>
        {
            var registros = producciones.Where(p => p.CowId == v.Id).ToList();
            return new VacaProduccionResponse
            {
                VacaId = v.Id,
                Raza = v.Race,
                TotalLitros = registros.Sum(p => p.LitrosProducidos),
                PromedioLitrosPorDia = registros.Count == 0 ? 0 : registros.Average(p => p.LitrosProducidos)
            };
        }).ToList();

        var promedio = porVaca.Count == 0 ? 0 : porVaca.Average(v => v.PromedioLitrosPorDia);
        return new ReporteVacasResponse
        {
            PromedioGeneralPorVaca = promedio,
            VacaQueMasProduce = porVaca.OrderByDescending(v => v.PromedioLitrosPorDia).ThenBy(v => v.VacaId).FirstOrDefault(),
            VacaQueMenosProduce = porVaca.OrderBy(v => v.PromedioLitrosPorDia).ThenBy(v => v.VacaId).FirstOrDefault(),
            VacasDebajoDelPromedio = porVaca.Where(v => v.PromedioLitrosPorDia < promedio).ToList()
        };
    }
}
