using Domain.Interfaces;

namespace Infrastructure;

public class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
