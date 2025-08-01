namespace Core.Enums.Risk;

/// <summary>
/// Risk categories based on PMI Risk Breakdown Structure (RBS)
/// </summary>
public enum RiskCategory
{
    Technical = 1,
    External = 2,
    Organizational = 3,
    ProjectManagement = 4,
    Commercial = 5,
    Environmental = 6,
    Safety = 7,
    Legal = 8,
    Financial = 9
}

/// <summary>
/// Risk type - Threat or Opportunity
/// </summary>
public enum RiskType
{
    Threat = 1,
    Opportunity = 2
}

/// <summary>
/// Risk probability levels for qualitative analysis
/// </summary>
public enum RiskProbability
{
    VeryLow = 1,    // 0-10%
    Low = 2,        // 10-30%
    Medium = 3,     // 30-50%
    High = 4,       // 50-70%
    VeryHigh = 5    // 70-100%
}

/// <summary>
/// Risk impact levels for qualitative analysis
/// </summary>
public enum RiskImpact
{
    VeryLow = 1,    // Minimal impact
    Low = 2,        // Minor impact
    Medium = 3,     // Moderate impact
    High = 4,       // Major impact
    VeryHigh = 5    // Severe impact
}

/// <summary>
/// Risk priority based on probability x impact matrix
/// </summary>
public enum RiskPriority
{
    VeryLow = 1,
    Low = 2,
    Medium = 3,
    High = 4,
    VeryHigh = 5
}

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

/// <summary>
/// Risk response action types
/// </summary>
public enum ResponseActionType
{
    Preventive = 1,
    Corrective = 2,
    Detective = 3,
    Contingent = 4
}

/// <summary>
/// Risk response status
/// </summary>
public enum RiskResponseStatus
{
    Planned = 1,
    InProgress = 2,
    Completed = 3,
    Evaluated = 4,
    Cancelled = 5
}