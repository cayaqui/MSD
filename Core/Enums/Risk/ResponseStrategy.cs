namespace Core.Enums.Risk;

/// <summary>
/// Risk response strategies
/// </summary>
public enum ResponseStrategy
{
    // For Threats
    Avoid = 1,
    Transfer = 2,
    Mitigate = 3,
    Accept = 4,

    // For Opportunities
    Exploit = 5,
    Share = 6,
    Enhance = 7,
    // Accept = 4 (same as threats)

    // Additional
    Escalate = 8,
    Contingent = 9
}
