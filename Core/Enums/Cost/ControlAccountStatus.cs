namespace Core.Enums.Cost;

/// <summary>
/// Control Account Status
/// </summary>
public enum ControlAccountStatus
{
    Open = 1,         // Initial state, can be edited
    Baselined = 2,    // Baselined, limited changes allowed
    InProgress = 3,   // Work has started
    Completed = 4,    // Work completed
    Closed = 5        // Financially closed

}
