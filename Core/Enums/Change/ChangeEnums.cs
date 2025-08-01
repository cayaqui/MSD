namespace Core.Enums.Change;

/// <summary>
/// Change order types
/// </summary>
public enum ChangeOrderType
{
    ScopeChange = 1,
    DesignChange = 2,
    ContractChange = 3,
    ScheduleChange = 4,
    CostChange = 5,
    Regulatory = 6,
    ClientRequest = 7,
    CorrectiveAction = 8
}

/// <summary>
/// Change order categories
/// </summary>
public enum ChangeOrderCategory
{
    Addition = 1,
    Deletion = 2,
    Modification = 3,
    Substitution = 4,
    Acceleration = 5,
    Deceleration = 6
}

/// <summary>
/// Change order priority levels
/// </summary>
public enum ChangeOrderPriority
{
    VeryLow = 1,
    Low = 2,
    Medium = 3,
    High = 4,
    Critical = 5
}

/// <summary>
/// Change order source
/// </summary>
public enum ChangeOrderSource
{
    Client = 1,
    Contractor = 2,
    Designer = 3,
    ProjectTeam = 4,
    Regulatory = 5,
    Vendor = 6,
    FieldCondition = 7,
    Other = 8
}

/// <summary>
/// Change order status workflow
/// </summary>
public enum ChangeOrderStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    Approved = 4,
    Rejected = 5,
    InProgress = 6,
    Implemented = 7,
    Closed = 8,
    Cancelled = 9,
    OnHold = 10
}

/// <summary>
/// Approval decision types
/// </summary>
public enum ApprovalDecision
{
    Pending = 1,
    Approved = 2,
    ApprovedWithConditions = 3,
    Rejected = 4,
    Deferred = 5,
    Escalated = 6
}

/// <summary>
/// Impact areas for change analysis
/// </summary>
public enum ImpactArea
{
    Cost = 1,
    Schedule = 2,
    Scope = 3,
    Quality = 4,
    Risk = 5,
    Resources = 6,
    Stakeholders = 7,
    Contracts = 8,
    Safety = 9,
    Environment = 10
}

/// <summary>
/// Impact severity levels
/// </summary>
public enum ImpactSeverity
{
    Negligible = 1,
    Minor = 2,
    Moderate = 3,
    Major = 4,
    Severe = 5
}