namespace Domain.Common;

/// <summary>
/// Interface for entities with a description property
/// </summary>
public interface IDescribable
{
    string? Description { get; set; }
}
