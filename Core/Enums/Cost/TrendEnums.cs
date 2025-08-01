namespace Core.Enums.Cost;

public enum TrendStatus
{
    Open = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Converted = 5,
    Closed = 6,
    OnHold = 7
}

public enum TrendCategory
{
    Scope = 1,
    Schedule = 2,
    Cost = 3,
    Quality = 4,
    Risk = 5,
    Technical = 6,
    Commercial = 7,
    Regulatory = 8,
    Environmental = 9,
    Safety = 10
}

public enum TrendType
{
    ClientRequest = 1,
    DesignChange = 2,
    SiteCondition = 3,
    Regulatory = 4,
    Error = 5,
    Optimization = 6,
    Risk = 7,
    Other = 8
}

public enum TrendPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum TrendImpactLevel
{
    Negligible = 1,
    Low = 2,
    Medium = 3,
    High = 4,
    Critical = 5
}

public enum TrendDecision
{
    Accept = 1,
    Reject = 2,
    Defer = 3,
    ConvertToChangeOrder = 4,
    IncludeInContingency = 5,
    NoActionRequired = 6
}