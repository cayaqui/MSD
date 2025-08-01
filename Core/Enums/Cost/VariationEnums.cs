namespace Core.Enums.Cost;

public enum VariationType
{
    Addition = 1,       // Scope addition
    Omission = 2,       // Scope reduction
    Substitution = 3,   // Material/method change
    Acceleration = 4,   // Schedule compression
    Delay = 5,          // Time extension
    RateAdjustment = 6, // Unit rate change
    QuantityChange = 7, // Quantity variation
    Provisional = 8     // Provisional sum adjustment
}

public enum VariationStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    Approved = 4,
    Rejected = 5,
    ClientRejected = 6,
    Disputed = 7,
    Implemented = 8,
    Closed = 9,
    Cancelled = 10
}

public enum VariationCategory
{
    ClientInstruction = 1,
    DesignChange = 2,
    SiteCondition = 3,
    Regulatory = 4,
    ErrorCorrection = 5,
    ValueEngineering = 6,
    Acceleration = 7,
    Suspension = 8,
    ForceMAjeure = 9,
    Other = 10
}

public enum PaymentMethod
{
    LumpSum = 1,
    UnitRate = 2,
    CostPlus = 3,
    Milestone = 4,
    TimeAndMaterial = 5,
    Provisional = 6
}

public enum AttachmentCategory
{
    ClientInstruction = 1,
    TechnicalDrawing = 2,
    Calculation = 3,
    Quotation = 4,
    Correspondence = 5,
    Photo = 6,
    Report = 7,
    Contract = 8,
    Other = 9
}