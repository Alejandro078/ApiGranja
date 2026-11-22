namespace Domain.Interfaces;

public interface IClock
{
    DateOnly Today { get; }
}
