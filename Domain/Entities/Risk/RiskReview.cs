using Core.Enums.Risk;
using Domain.Common;
using Domain.Entities.Security;
using System;
using System.Threading.Tasks;

namespace Domain.Entities.Risk;

/// <summary>
/// Risk Review for periodic risk assessment updates
/// </summary>
public class RiskReview : BaseEntity, IAuditable
{
    // Basic Information
    public Guid RiskId { get; private set; }
    public DateTime ReviewDate { get; private set; }
    public string ReviewedById { get; private set; } = string.Empty;

    // Previous Values (for tracking changes)
    public RiskProbability PreviousProbability { get; private set; }
    public RiskImpact PreviousImpact { get; private set; }
    public RiskStatus PreviousStatus { get; private set; }

    // New Values
    public RiskProbability NewProbability { get; private set; }
    public RiskImpact NewImpact { get; private set; }
    public RiskStatus NewStatus { get; private set; }

    // Review Details
    public string ReviewNotes { get; private set; } = string.Empty;
    public string? ChangesIdentified { get; private set; }
    public string? RecommendedActions { get; private set; }

    // Navigation Properties
    public Risk Risk { get; private set; } = null!;
    public User ReviewedBy { get; private set; } = null!;

    // Constructor for EF Core
    private RiskReview() { }

    public RiskReview(
        Guid riskId,
        string reviewedById,
        RiskProbability previousProbability,
        RiskImpact previousImpact,
        RiskStatus previousStatus,
        RiskProbability newProbability,
        RiskImpact newImpact,
        RiskStatus newStatus,
        string reviewNotes)
    {
        RiskId = riskId;
        ReviewedById = reviewedById ?? throw new ArgumentNullException(nameof(reviewedById));
        ReviewDate = DateTime.UtcNow;

        PreviousProbability = previousProbability;
        PreviousImpact = previousImpact;
        PreviousStatus = previousStatus;

        NewProbability = newProbability;
        NewImpact = newImpact;
        NewStatus = newStatus;

        ReviewNotes = reviewNotes ?? throw new ArgumentNullException(nameof(reviewNotes));

        CreatedAt = DateTime.UtcNow;
    }

    public void AddRecommendations(string recommendations)
    {
        RecommendedActions = recommendations;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IdentifyChanges(string changes)
    {
        ChangesIdentified = changes;
        UpdatedAt = DateTime.UtcNow;
    }
}