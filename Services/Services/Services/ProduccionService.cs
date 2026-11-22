using Domain;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Services.Dtos;
using Services.Interfaces;

namespace Services.Services;

public class ProduccionService : IProduccionService
{
    private readonly IGranjaRepository _granjas;
    private readonly ICowRepository _vacas;
    private readonly IProduccionLecheRepository _producciones;
    private readonly ILoteRepository _lotes;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public ProduccionService(IGranjaRepository granjas, ICowRepository vacas,
        IProduccionLecheRepository producciones, ILoteRepository lotes,
        IUnitOfWork uow, IClock clock)
    {
        _granjas = granjas;
        _vacas = vacas;
        _producciones = producciones;
        _lotes = lotes;
        _uow = uow;
        _clock = clock;
    }

    public async Task<ProduccionResponse> RegistrarProduccionDiariaAsync(ProduccionRequest request, CancellationToken cancellationToken = default)
    {
        var fecha = request.Fecha ?? _clock.Today;
        var granja = await _granjas.ObtenerAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Primero debe consultar o configurar la granja en /api/granja.");
        var vacas = await _vacas.ObtenerTodosAsync(cancellationToken);
        if (vacas.Count != granja.NumeroVacas)
            throw new ConflictoOperacionException("El número de vacas registradas no coincide con la configuración de la granja.");

        var existentes = await _producciones.ObtenerPorFechaAsync(fecha, cancellationToken);
        if (existentes.Count > 0)
            throw new ConflictoOperacionException("La producción ya fue registrada para una o más vacas en esa fecha.");

        foreach (var vaca in vacas)
            await _producciones.AgregarAsync(new ProduccionLeche(vaca.Id, fecha, granja.LitrosPorVacaDia), cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);
        return new ProduccionResponse
        {
            Fecha = fecha,
            NumeroVacas = vacas.Count,
            LitrosPorVaca = granja.LitrosPorVacaDia,
            LitrosTotales = granja.CalcularProduccionDiaria()
        };
    }

    public async Task<TransformacionResponse> TransformarLecheAsync(TransformacionRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Litros <= 0) throw new ArgumentException("Los litros a transformar deben ser mayores a cero.");
        if (!Enum.IsDefined(request.TipoProducto)) throw new ArgumentException("Tipo de producto inválido.");

        var fecha = request.Fecha ?? _clock.Today;
        var granja = await _granjas.ObtenerAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Primero debe configurar la granja en /api/granja.");
        var producciones = await _producciones.ObtenerPorFechaAsync(fecha, cancellationToken);
        var disponible = producciones.Sum(p => p.LitrosNoAprovechados);
        if (request.Litros > disponible)
            throw new LecheInsuficienteException(request.Litros, disponible);

        var restante = request.Litros;
        foreach (var produccion in producciones)
        {
            if (restante <= 0) break;
            var tomar = Math.Min(produccion.LitrosNoAprovechados, restante);
            if (tomar > 0) produccion.RegistrarTransformacion(tomar);
            restante -= tomar;
        }

        var rendimiento = request.TipoProducto == TipoProducto.Queso ? granja.RendimientoQueso : granja.RendimientoYogurt;
        var vidaUtil = request.TipoProducto == TipoProducto.Queso ? granja.VidaUtilQueso : granja.VidaUtilYogurt;
        var cantidad = request.Litros / rendimiento;
        var lote = new LoteProducto(request.TipoProducto, fecha, cantidad, vidaUtil);
        await _lotes.AgregarAsync(lote, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new TransformacionResponse
        {
            LoteId = lote.Id,
            FechaProduccion = fecha,
            FechaVencimiento = lote.FechaVencimiento,
            TipoProducto = request.TipoProducto,
            LitrosUsados = request.Litros,
            CantidadObtenida = cantidad
        };
    }
}
