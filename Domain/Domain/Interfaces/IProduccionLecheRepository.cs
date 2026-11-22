namespace Domain.Interfaces;

public interface IProduccionLecheRepository : IRepository<ProduccionLeche>
{
    Task<List<ProduccionLeche>> ObtenerPorFechaAsync(DateOnly fecha, CancellationToken cancellationToken = default);
    Task<List<ProduccionLeche>> ObtenerEntreFechasAsync(DateOnly? desde, DateOnly? hasta, CancellationToken cancellationToken = default);
    Task<bool> TieneProduccionAsync(int cowId, CancellationToken cancellationToken = default);
}
