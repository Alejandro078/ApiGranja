namespace Domain.Interfaces;

public interface IVentaRepository : IRepository<Venta>
{
    Task<List<Venta>> ObtenerEntreFechasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default);
}
