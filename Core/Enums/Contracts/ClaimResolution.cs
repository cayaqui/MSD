namespace Core.Enums.Contracts;

public enum ClaimResolution
{
    Pending = 1,
    FullyAccepted = 2,
    PartiallyAccepted = 3,
    Rejected = 4,
    Negotiated = 5,
    Mediated = 6,
    Arbitrated = 7,
    Litigated = 8,
    Withdrawn = 9,
    TimeBarred = 10
}