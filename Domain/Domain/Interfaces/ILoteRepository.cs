using Domain.Enums;

namespace Domain.Interfaces;

public interface ILoteRepository : IRepository<LoteProducto>
{
    Task<List<LoteProducto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<List<LoteProducto>> ObtenerVendiblesAsync(TipoProducto tipo, DateOnly fecha, CancellationToken cancellationToken = default);
}
