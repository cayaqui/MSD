namespace Core.Enums.Change;

public enum ChangeRequestStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    PendingApproval = 4,
    Approved = 5,
    Rejected = 6,
    ClientRejected = 7,
    ReadyToImplement = 8,
    Converted = 9,
    Implemented = 10,
    Closed = 11,
    Cancelled = 12,
    OnHold = 13
}

public enum ChangeRequestType
{
    Technical = 1,
    Commercial = 2,
    Schedule = 3,
    Administrative = 4,
    Safety = 5,
    Quality = 6,
    Regulatory = 7,
    Mixed = 8
}

public enum ChangeRequestCategory
{
    ScopeAddition = 1,
    ScopeReduction = 2,
    DesignModification = 3,
    SpecificationChange = 4,
    SiteCondition = 5,
    Acceleration = 6,
    Delay = 7,
    CostOptimization = 8,
    QualityImprovement = 9,
    SafetyRequirement = 10,
    RegulatoryCompliance = 11,
    ErrorCorrection = 12,
    ClientRequest = 13,
    ForceMAjeure = 14
}

public enum ChangeRequestSource
{
    Client = 1,
    Contractor = 2,
    Designer = 3,
    ProjectTeam = 4,
    Regulatory = 5,
    ThirdParty = 6,
    Internal = 7
}

public enum ChangeRequestPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4,
    Critical = 5
}

public enum ChangeRequestImpactLevel
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum CostImpactType
{
    Increase = 1,
    Decrease = 2,
    Neutral = 3,
    Transfer = 4,
    Provisional = 5
}