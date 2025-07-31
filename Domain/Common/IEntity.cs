namespace Domain.Common;

/// <summary>
/// Base entity interface with ID
/// </summary>
public interface IEntity
{
    Guid Id { get; set; }
}
