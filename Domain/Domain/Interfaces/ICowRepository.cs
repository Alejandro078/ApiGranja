namespace Domain.Interfaces;

public interface ICowRepository : IRepository<Cow>
{
    Task<List<Cow>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task EliminarAsync(Cow vaca, CancellationToken cancellationToken = default);
}
