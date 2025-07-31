namespace Core.Enums.Cost;

/// <summary>
/// Commitment status
/// </summary>
public enum CommitmentStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Active = 4,
    PartiallyInvoiced = 5,
    FullyInvoiced = 6,
    Closed = 7,
    Cancelled = 8,
}
