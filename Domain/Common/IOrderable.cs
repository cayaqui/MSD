namespace Domain.Common;

/// <summary>
/// Interface for entities that can be ordered
/// </summary>
public interface IOrderable
{
    int Order { get; set; }
}
