namespace Domain.Interfaces;

public interface IGranjaRepository : IRepository<Granja>
{
    Task<Granja?> ObtenerAsync(CancellationToken cancellationToken = default);
}
