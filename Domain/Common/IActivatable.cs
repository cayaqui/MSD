namespace Domain.Common;

/// <summary>
/// Interface for entities that can be activated/deactivated
/// </summary>
public interface IActivatable
{
    bool IsActive { get; set; }
}