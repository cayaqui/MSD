namespace Core.Enums.Risk;

/// <summary>
/// Risk status in the risk management process
/// </summary>
public enum RiskStatus
{
    Identified = 1,
    Analyzed = 2,
    ResponsePlanned = 3,
    InProgress = 4,
    Occurred = 5,
    Closed = 6,
    Escalated = 7
}
