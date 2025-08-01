namespace Core.Enums.Projects;

#region Contractor Enums
/// <summary>
/// Contractor Type
/// </summary>
public enum ContractorType
{
    Supplier = 1,
    Subcontractor = 2,
    ServiceProvider = 3,
    Consultant = 4,
    Manufacturer = 5,
    Distributor = 6,
    Other = 99
}

/// <summary>
/// Contractor Classification
/// </summary>
public enum ContractorClassification
{
    Preferred = 1,    // High performance, priority
    Standard = 2,     // Normal classification
    Restricted = 3,   // Limited use, performance issues
    Strategic = 4     // Strategic partner
}

/// <summary>
/// Contractor Status
/// </summary>
public enum ContractorStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Blacklisted = 4,
    UnderReview = 5,
    Pending = 6
}
#endregion

#region Company Enums
/// <summary>
/// Company Type
/// </summary>
public enum CompanyType
{
    Owner = 1,
    Contractor = 2,
    Consultant = 3,
    Supplier = 4,
    Partner = 5
}

/// <summary>
/// Company Status
/// </summary>
public enum CompanyStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3
}
#endregion

#region Operation Enums
/// <summary>
/// Operation Type
/// </summary>
public enum OperationType
{
    BusinessUnit = 1,
    Division = 2,
    Department = 3,
    Region = 4,
    Branch = 5
}

/// <summary>
/// Operation Status
/// </summary>
public enum OperationStatus
{
    Active = 1,
    Inactive = 2,
    Planning = 3,
    Closing = 4
}
#endregion

#region Project Enums
/// <summary>
/// Project Type
/// </summary>
public enum ProjectType
{
    EPC = 1,              // Engineering, Procurement, Construction
    EPCM = 2,             // EPC + Management
    DesignBuild = 3,
    Construction = 4,
    Engineering = 5,
    Maintenance = 6,
    Consulting = 7,
    TurnKey = 8,
    BOT = 9,              // Build, Operate, Transfer
    PPP = 10              // Public-Private Partnership
}


/// <summary>
/// Project Complexity
/// </summary>
public enum ProjectComplexity
{
    Low = 1,
    Medium = 2,
    High = 3,
    VeryHigh = 4
}
#endregion

#region Discipline Enums
/// <summary>
/// Discipline Type
/// </summary>
public enum DisciplineType
{
    Civil = 1,
    Mechanical = 2,
    Electrical = 3,
    Instrumentation = 4,
    Process = 5,
    Piping = 6,
    Structural = 7,
    Architecture = 8,
    HSE = 9,              // Health, Safety, Environment
    QualityControl = 10,
    ProjectControl = 11,
    Other = 99
}
#endregion

#region Document Enums
/// <summary>
/// Document Type
/// </summary>
public enum DocumentType
{
    Drawing = 1,
    Specification = 2,
    Procedure = 3,
    Report = 4,
    Datasheet = 5,
    Calculation = 6,
    Certificate = 7,
    Correspondence = 8,
    Minutes = 9,
    Transmittal = 10,
    RFI = 11,             // Request for Information
    NCR = 12,             // Non-Conformance Report
    Other = 99
}

/// <summary>
/// Document Status
/// </summary>
public enum DocumentStatus
{
    Draft = 1,
    ForReview = 2,
    ForApproval = 3,
    Approved = 4,
    ForConstruction = 5,
    AsBuilt = 6,
    Superseded = 7,
    Cancelled = 8
}
#endregion