namespace Core.Enums.Cost;

/// <summary>
/// BudgetItem status according to PMBOK
/// </summary>
public enum BudgetItemStatus
{
    Draft = 1,
    UnderReview = 2,
    Approved = 3,
    Active = 4,
    Frozen = 5,
    Closed = 6,
    Rejected = 7,
}

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

/// <summary>
/// Measurement method for progress tracking
/// </summary>
public enum MeasurementMethod
{
    PercentComplete = 1,
    WeightedMilestone = 2,
    UnitsCompleted = 3,
    LevelOfEffort = 4,
    ApportionedEffort = 5,
    EarnedStandards = 6
}


/// <summary>
/// Cost Type classification
/// </summary>
public enum CostType2
{
    Direct = 1,
    Indirect = 2,
    Fixed = 3,
    Variable = 4,
    Capital = 5,
    Operating = 6
}

/// <summary>
/// Cost Item Status
/// </summary>
public enum CostItemStatus
{
    Planned = 1,
    Committed = 2,
    Actual = 3,
    Forecast = 4,
    Closed = 5
}

/// <summary>
/// EVM Period Type
/// </summary>
public enum EVMPeriodType
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    Biannual = 5,
    Yearly = 6
}


/// <summary>
/// Control Account Role
/// </summary>
public enum ControlAccountRole
{
    Manager = 1,      // CAM - Control Account Manager
    Analyst = 2,      // Cost/Schedule Analyst
    Approver = 3,     // Budget Approver
    Viewer = 4        // Read-only access
}
