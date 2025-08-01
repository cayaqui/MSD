using System;
using Domain.Common;
using Core.Enums.ChangeManagement;

namespace Domain.Entities.ChangeManagement;

/// <summary>
/// Detailed impact analysis for Change Orders
/// </summary>
public class ChangeOrderImpact : BaseEntity, IAuditable
{
    // Basic Information
    public Guid ChangeOrderId { get; private set; }
    public ImpactArea Area { get; private set; }
    public ImpactSeverity Severity { get; private set; }

    // Impact Details
    public string Description { get; private set; } = string.Empty;
    public decimal? CostImpact { get; private set; }
    public int? ScheduleImpactDays { get; private set; }
    public string? AffectedWBSCodes { get; private set; } // JSON array
    public string? AffectedStakeholders { get; private set; } // JSON array

    // Mitigation
    public string? MitigationPlan { get; private set; }
    public decimal? MitigationCost { get; private set; }

    // Navigation Properties
    public ChangeOrder ChangeOrder { get; private set; } = null!;

    // Constructor for EF Core
    private ChangeOrderImpact() { }

    public ChangeOrderImpact(
        Guid changeOrderId,
        ImpactArea area,
        ImpactSeverity severity,
        string description)
    {
        ChangeOrderId = changeOrderId;
        Area = area;
        Severity = severity;
        Description = description ?? throw new ArgumentNullException(nameof(description));

        CreatedAt = DateTime.UtcNow;
    }

    public void QuantifyImpact(decimal? costImpact, int? scheduleImpactDays)
    {
        CostImpact = costImpact;
        ScheduleImpactDays = scheduleImpactDays;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IdentifyAffectedElements(string? wbsCodes, string? stakeholders)
    {
        AffectedWBSCodes = wbsCodes;
        AffectedStakeholders = stakeholders;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PlanMitigation(string plan, decimal? cost)
    {
        MitigationPlan = plan ?? throw new ArgumentNullException(nameof(plan));
        MitigationCost = cost;
        UpdatedAt = DateTime.UtcNow;
    }
}