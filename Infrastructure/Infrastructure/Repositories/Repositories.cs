using Domain;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CowRepository : ICowRepository
{
    private readonly AppDbContext _context;
    public CowRepository(AppDbContext context) => _context = context;
    public Task<List<Cow>> ObtenerTodosAsync(CancellationToken cancellationToken = default) =>
        _context.Cows.OrderBy(c => c.Id).ToListAsync(cancellationToken);
    public Task AgregarAsync(Cow entidad, CancellationToken cancellationToken = default) =>
        _context.Cows.AddAsync(entidad, cancellationToken).AsTask();
    public Task EliminarAsync(Cow vaca, CancellationToken cancellationToken = default)
    {
        _context.Cows.Remove(vaca);
        return Task.CompletedTask;
    }
}

public class GranjaRepository : IGranjaRepository
{
    private readonly AppDbContext _context;
    public GranjaRepository(AppDbContext context) => _context = context;
    public Task<Granja?> ObtenerAsync(CancellationToken cancellationToken = default) =>
        _context.Granjas.OrderBy(g => g.Id).FirstOrDefaultAsync(cancellationToken);
    public Task AgregarAsync(Granja entidad, CancellationToken cancellationToken = default) =>
        _context.Granjas.AddAsync(entidad, cancellationToken).AsTask();
}

public class ProduccionLecheRepository : IProduccionLecheRepository
{
    private readonly AppDbContext _context;
    public ProduccionLecheRepository(AppDbContext context) => _context = context;
    public Task<List<ProduccionLeche>> ObtenerPorFechaAsync(DateOnly fecha, CancellationToken cancellationToken = default) =>
        _context.Producciones.Where(p => p.Fecha == fecha).OrderBy(p => p.CowId).ToListAsync(cancellationToken);
    public Task<List<ProduccionLeche>> ObtenerEntreFechasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default)
    {
        var query = _context.Producciones.AsQueryable();
        if (desde.HasValue) query = query.Where(p => p.Fecha >= desde.Value);
        if (hasta.HasValue) query = query.Where(p => p.Fecha <= hasta.Value);
        return query.OrderBy(p => p.Fecha).ThenBy(p => p.CowId).ToListAsync(cancellationToken);
    }
    public Task<bool> TieneProduccionAsync(int cowId, CancellationToken cancellationToken = default) =>
        _context.Producciones.AnyAsync(p => p.CowId == cowId, cancellationToken);
    public Task AgregarAsync(ProduccionLeche entidad, CancellationToken cancellationToken = default) =>
        _context.Producciones.AddAsync(entidad, cancellationToken).AsTask();
}

public class LoteRepository : ILoteRepository
{
    private readonly AppDbContext _context;
    public LoteRepository(AppDbContext context) => _context = context;
    public Task<List<LoteProducto>> ObtenerTodosAsync(CancellationToken cancellationToken = default) =>
        _context.Lotes.OrderBy(l => l.FechaProduccion).ThenBy(l => l.Id).ToListAsync(cancellationToken);
    public async Task<List<LoteProducto>> ObtenerVendiblesAsync(TipoProducto tipo, DateOnly fecha, CancellationToken cancellationToken = default)
    {
        var lotes = await _context.Lotes
            .Where(l => l.Tipo == tipo && l.FechaVencimiento >= fecha)
            .OrderBy(l => l.FechaProduccion).ThenBy(l => l.Id)
            .ToListAsync(cancellationToken);
        return lotes.Where(l => l.CantidadDisponible > 0).ToList();
    }
    public Task AgregarAsync(LoteProducto entidad, CancellationToken cancellationToken = default) =>
        _context.Lotes.AddAsync(entidad, cancellationToken).AsTask();
}

public class VentaRepository : IVentaRepository
{
    private readonly AppDbContext _context;
    public VentaRepository(AppDbContext context) => _context = context;
    public Task<List<Venta>> ObtenerEntreFechasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default)
    {
        var query = _context.Ventas.Include(v => v.Detalles).ThenInclude(d => d.Lote).AsQueryable();
        if (desde.HasValue) query = query.Where(v => v.Fecha >= desde.Value);
        if (hasta.HasValue) query = query.Where(v => v.Fecha <= hasta.Value);
        return query.OrderBy(v => v.Fecha).ThenBy(v => v.Id).ToListAsync(cancellationToken);
    }
    public Task AgregarAsync(Venta entidad, CancellationToken cancellationToken = default) =>
        _context.Ventas.AddAsync(entidad, cancellationToken).AsTask();
}
