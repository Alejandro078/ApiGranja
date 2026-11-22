namespace Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task AgregarAsync(T entidad, CancellationToken cancellationToken = default);
}
