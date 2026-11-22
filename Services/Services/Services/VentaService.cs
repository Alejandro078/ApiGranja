using Domain;
using Domain.Exceptions;
using Domain.Interfaces;
using Services.Dtos;
using Services.Interfaces;

namespace Services.Services;

public class VentaService : IVentaService
{
    private readonly ILoteRepository _lotes;
    private readonly IVentaRepository _ventas;
    private readonly IGranjaRepository _granjas;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public VentaService(ILoteRepository lotes, IVentaRepository ventas, IGranjaRepository granjas, IUnitOfWork uow, IClock clock)
    {
        _lotes = lotes;
        _ventas = ventas;
        _granjas = granjas;
        _uow = uow;
        _clock = clock;
    }

    public async Task<VentaResponse> RegistrarVentaAsync(VentaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        if (!Enum.IsDefined(request.TipoProducto)) throw new ArgumentException("Tipo de producto inválido.");

        var fecha = request.Fecha ?? _clock.Today;
        var granja = await _granjas.ObtenerAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Primero debe configurar la granja en /api/granja.");
        var lotes = await _lotes.ObtenerVendiblesAsync(request.TipoProducto, fecha, cancellationToken);

        // Se valida la cantidad total antes de descontar cualquier lote: la venta es atómica.
        if (lotes.Sum(l => l.CantidadDisponible) < request.Cantidad)
            throw new InventarioInsuficienteException(request.TipoProducto, request.Cantidad);

        var precio = request.TipoProducto == Domain.Enums.TipoProducto.Queso ? granja.PrecioQueso : granja.PrecioYogurt;
        var venta = new Venta(fecha, request.TipoProducto, request.Cantidad, precio);
        var restante = request.Cantidad;
        foreach (var lote in lotes)
        {
            if (restante <= 0) break;
            var tomar = Math.Min(lote.CantidadDisponible, restante);
            lote.Descontar(tomar);
            venta.AgregarDetalle(lote, tomar);
            restante -= tomar;
        }

        await _ventas.AgregarAsync(venta, cancellationToken);
        // Una sola llamada a SaveChanges guarda la venta y todos los descuentos en una transacción EF Core.
        await _uow.SaveChangesAsync(cancellationToken);
        return MapearVenta(venta);
    }

    public async Task<List<VentaResponse>> ObtenerVentasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default)
    {
        var ventas = await _ventas.ObtenerEntreFechasAsync(desde, hasta, cancellationToken);
        return ventas.Select(MapearVenta).ToList();
    }

    internal static VentaResponse MapearVenta(Venta venta) => new()
    {
        Id = venta.Id,
        Fecha = venta.Fecha,
        TipoProducto = venta.TipoProducto,
        Cantidad = venta.Cantidad,
        PrecioUnitario = venta.PrecioUnitario,
        Total = venta.Total,
        Detalle = venta.Detalles.Select(d => new VentaDetalleResponse
        {
            LoteId = d.LoteId,
            FechaProduccion = d.Lote?.FechaProduccion ?? default,
            Cantidad = d.Cantidad
        }).ToList()
    };
}
