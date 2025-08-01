namespace Core.Enums.Cost;

#region Commitment Enums

/// <summary>
/// Commitment Item Status
/// </summary>
public enum CommitmentItemStatus
{
    Active = 1,
    PartiallyDelivered = 2,
    FullyDelivered = 3,
    PartiallyInvoiced = 4,
    FullyInvoiced = 5,
    Completed = 6,
    Cancelled = 7,
    Locked = 8
}

#endregion
