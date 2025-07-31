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

    Planning = 1,
    Baselined = 2,
    InProgress = 3,
    OnHold = 4,
    Completed = 5,
    Closed = 6,
    UnderReview = 7,

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
/// Cost Category
/// </summary>
public enum CostCategory
{
    Labor = 1,
    Material = 2,
    Equipment = 3,
    Subcontract = 4,
    Travel = 5,
    Other = 6,
    Overhead = 7,
    Contingency = 8
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
    Weekly = 1,
    Monthly = 2,
    Quarterly = 3
}

/// <summary>
/// EVM Status based on performance indices
/// </summary>
public enum EVMStatus
{
    OnTrack = 1,      // CPI & SPI >= 0.95
    AtRisk = 2,       // CPI & SPI >= 0.90
    OffTrack = 3,     // CPI or SPI < 0.90
    Critical = 4      // Severe issues
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
