namespace Core.Enums.Cost;

/// <summary>
/// Commitment types for cost control
/// </summary>
public enum CommitmentType
{
    PurchaseOrder = 1,
    Contract = 2,
    ServiceAgreement = 3,
    LaborAgreement = 4,
    LeaseAgreement = 5,
    AccountAdjustment = 6,
    ChangeOrder = 7,
    Retention = 8,
    Other = 99,
}
