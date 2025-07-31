namespace Core.Enums.Cost;

/// <summary>
/// Invoice status
/// </summary>
public enum InvoiceStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    Approved = 4,
    Rejected = 5,
    Paid = 6,
    PartiallyPaid = 7,
    Cancelled = 8,
    Overdue = 9,
}
