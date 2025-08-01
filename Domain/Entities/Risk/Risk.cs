
using Core.Enums.Risk;
using Domain.Common;
using Domain.Entities.Projects;
using Domain.Entities.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Entities.Risk;

/// <summary>
/// Risk entity following PMI Risk Management standards
/// Format: R-XXX-###
/// </summary>
public class Risk : BaseEntity, IAuditable, ISoftDelete, IActivatable
{
    // Basic Information
    public string RiskCode { get; private set; } = string.Empty; // R-XXX-###
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Classification
    public RiskCategory Category { get; private set; }
    public RiskType Type { get; private set; } // Threat or Opportunity
    public string? RBS { get; private set; } // Risk Breakdown Structure code

    // Qualitative Analysis
    public RiskProbability Probability { get; private set; }
    public RiskImpact Impact { get; private set; }
    public RiskPriority Priority { get; private set; }

    // Quantitative Analysis (Optional)
    public decimal? CostImpact { get; private set; }
    public int? ScheduleImpactDays { get; private set; }
    public decimal? ProbabilityPercentage { get; private set; } // For Monte Carlo

    // Risk Response
    public RiskStatus Status { get; private set; }
    public ResponseStrategy? Strategy { get; private set; }
    public string? ResponsePlan { get; private set; }
    public decimal? ContingencyReserve { get; private set; }
    public string? FallbackPlan { get; private set; }

    // Triggers and Monitoring
    public string? Triggers { get; private set; } // Warning signs
    public string? ResidualRiskDescription { get; private set; }
    public RiskImpact? ResidualImpact { get; private set; }
    public RiskProbability? ResidualProbability { get; private set; }

    // Assignment
    public Guid ProjectId { get; private set; }
    public Guid? WBSElementId { get; private set; }
    public Guid? ControlAccountId { get; private set; }
    public string OwnerId { get; private set; } = string.Empty; // Risk Owner
    public string? ActioneeId { get; private set; } // Person implementing response

    // Important Dates
    public DateTime IdentifiedDate { get; private set; }
    public DateTime? ResponseDueDate { get; private set; }
    public DateTime? ReviewDate { get; private set; }
    public DateTime? ClosedDate { get; private set; }

    // Calculated Fields
    public decimal RiskScore => (int)Probability * (int)Impact;
    public decimal ResidualRiskScore =>
        ResidualProbability.HasValue && ResidualImpact.HasValue
            ? (int)ResidualProbability.Value * (int)ResidualImpact.Value
            : 0;

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Activatable
    public bool IsActive { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public WBSElement? WBSElement { get; private set; }
    public ControlAccount? ControlAccount { get; private set; }
    public User Owner { get; private set; } = null!;
    public User? Actionee { get; private set; }
    public ICollection<RiskResponse> Responses { get; private set; } = new List<RiskResponse>();
    public ICollection<RiskReview> Reviews { get; private set; } = new List<RiskReview>();

    // Constructor for EF Core
    private Risk() { }

    public Risk(
        string riskCode,
        string title,
        string description,
        Guid projectId,
        string ownerId,
        RiskCategory category,
        RiskType type)
    {
        RiskCode = riskCode ?? throw new ArgumentNullException(nameof(riskCode));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        ProjectId = projectId;
        OwnerId = ownerId ?? throw new ArgumentNullException(nameof(ownerId));
        Category = category;
        Type = type;

        Status = RiskStatus.Identified;
        Probability = RiskProbability.Medium;
        Impact = RiskImpact.Medium;
        Priority = RiskPriority.Medium;
        IdentifiedDate = DateTime.UtcNow;
        IsActive = true;

        ValidateRiskCode();
    }

    // Domain Methods
    public void PerformQualitativeAnalysis(
        RiskProbability probability,
        RiskImpact impact,
        string? triggers = null)
    {
        Probability = probability;
        Impact = impact;
        Triggers = triggers;

        // Update priority based on risk matrix
        Priority = CalculatePriority(probability, impact);
        Status = RiskStatus.Analyzed;

        UpdatedAt = DateTime.UtcNow;
    }

    public void PerformQuantitativeAnalysis(
        decimal? costImpact,
        int? scheduleImpactDays,
        decimal? probabilityPercentage)
    {
        if (Status < RiskStatus.Analyzed)
            throw new InvalidOperationException("Must perform qualitative analysis first");

        CostImpact = costImpact;
        ScheduleImpactDays = scheduleImpactDays;
        ProbabilityPercentage = probabilityPercentage;

        UpdatedAt = DateTime.UtcNow;
    }

    public void PlanResponse(
        ResponseStrategy strategy,
        string responsePlan,
        decimal? contingencyReserve = null,
        string? fallbackPlan = null)
    {
        if (Status < RiskStatus.Analyzed)
            throw new InvalidOperationException("Must analyze risk before planning response");

        Strategy = strategy;
        ResponsePlan = responsePlan ?? throw new ArgumentNullException(nameof(responsePlan));
        ContingencyReserve = contingencyReserve;
        FallbackPlan = fallbackPlan;
        Status = RiskStatus.ResponsePlanned;

        UpdatedAt = DateTime.UtcNow;
    }

    public void ImplementResponse(string actioneeId, DateTime dueDate)
    {
        if (Status < RiskStatus.ResponsePlanned)
            throw new InvalidOperationException("Must plan response before implementation");

        ActioneeId = actioneeId ?? throw new ArgumentNullException(nameof(actioneeId));
        ResponseDueDate = dueDate;
        Status = RiskStatus.InProgress;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateResidualRisk(
        RiskProbability residualProbability,
        RiskImpact residualImpact,
        string? residualDescription)
    {
        ResidualProbability = residualProbability;
        ResidualImpact = residualImpact;
        ResidualRiskDescription = residualDescription;

        UpdatedAt = DateTime.UtcNow;
    }

    public void CloseRisk(string? closureNotes = null)
    {
        Status = RiskStatus.Closed;
        ClosedDate = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(closureNotes))
        {
            Description += $"\n\nClosure Notes: {closureNotes}";
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReactivateRisk(string reason)
    {
        if (Status != RiskStatus.Closed)
            throw new InvalidOperationException("Can only reactivate closed risks");

        Status = RiskStatus.InProgress;
        ClosedDate = null;
        IsActive = true;
        Description += $"\n\nReactivated: {reason}";

        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToWBS(Guid wbsElementId)
    {
        WBSElementId = wbsElementId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToControlAccount(Guid controlAccountId)
    {
        ControlAccountId = controlAccountId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOwner(string newOwnerId)
    {
        OwnerId = newOwnerId ?? throw new ArgumentNullException(nameof(newOwnerId));
        UpdatedAt = DateTime.UtcNow;
    }

    // Private Methods
    private void ValidateRiskCode()
    {
        // Format: R-XXX-###
        if (!System.Text.RegularExpressions.Regex.IsMatch(RiskCode, @"^R-\d{3}-\d{3}$"))
            throw new ArgumentException("Risk code must follow format R-XXX-### (e.g., R-001-001)");
    }

    private RiskPriority CalculatePriority(RiskProbability probability, RiskImpact impact)
    {
        var score = (int)probability * (int)impact;

        return score switch
        {
            >= 15 => RiskPriority.VeryHigh,
            >= 10 => RiskPriority.High,
            >= 6 => RiskPriority.Medium,
            >= 3 => RiskPriority.Low,
            _ => RiskPriority.VeryLow
        };
    }
}