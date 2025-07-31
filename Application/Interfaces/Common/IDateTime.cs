namespace Application.Interfaces.Common;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
