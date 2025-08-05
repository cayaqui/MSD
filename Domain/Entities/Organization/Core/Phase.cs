using Domain.Common;
using Domain.Entities.Cost.Control;
using Core.Enums.Projects;
using System;
using System.Collections.Generic;
using Domain.Entities.Progress;
using Domain.Entities.WBS;

namespace Domain.Entities.Organization.Core;

/// <summary>
/// Project Phase entity based on PMBOK lifecycle
/// Represents major phases like Initiation, Planning, Execution, Monitoring & Control, Closing
/// </summary>
public class Phase : BaseEntity, IAuditable, ISoftDelete, ICodeEntity, INamedEntity, IActivatable
{
    // Basic Information
    public string Code { get; set; } = string.Empty; // Format: XXX-YY (Project-Phase)
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Foreign Keys
    public Guid ProjectId { get; set; }

    // Phase Information
    public int SequenceNumber { get; set; }
    public PhaseType PhaseType { get; set; }
    public PhaseStatus Status { get; set; }

    // Schedule
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public DateTime? BaselineStartDate { get; set; }
    public DateTime? BaselineEndDate { get; set; }

    // Budget
    public decimal PlannedBudget { get; set; }
    public decimal ApprovedBudget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }

    // Progress
    public decimal ProgressPercentage { get; set; }
    public decimal WeightFactor { get; set; } // Weight in overall project progress

    // Deliverables and Gates
    public string? KeyDeliverables { get; set; } // JSON array of deliverables
    public bool RequiresGateApproval { get; set; }
    public DateTime? GateApprovalDate { get; set; }
    public string? GateApprovedBy { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Activatable
    public bool IsActive { get; set; }

    // Navigation Properties
    public Project Project { get; set; } = null!;
    public ICollection<WBSElement> WBSElements { get; set; } = new List<WBSElement>();
    public ICollection<ControlAccount> ControlAccounts { get; set; } = new List<ControlAccount>();
    public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    public ICollection<PlanningPackage> PlanningPackages { get; set; } = new List<PlanningPackage>();

    // Constructor for EF Core
    private Phase() { }

    public Phase(string code, string name, Guid projectId, PhaseType phaseType, int sequenceNumber)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProjectId = projectId;
        PhaseType = phaseType;
        SequenceNumber = sequenceNumber;
        Status = PhaseStatus.NotStarted;
        IsActive = true;
        WeightFactor = 1.0m;
    }

    // Domain Methods
    public void Start(string userId)
    {
        if (Status != PhaseStatus.NotStarted)
            throw new InvalidOperationException("Phase can only be started from NotStarted status");

        Status = PhaseStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string userId)
    {
        if (Status != PhaseStatus.InProgress)
            throw new InvalidOperationException("Phase can only be completed from InProgress status");

        Status = PhaseStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        ProgressPercentage = 100;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApproveGate(string approvedBy)
    {
        if (!RequiresGateApproval)
            throw new InvalidOperationException("This phase does not require gate approval");

        GateApprovalDate = DateTime.UtcNow;
        GateApprovedBy = approvedBy;
        UpdatedBy = approvedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal percentage, decimal actualCost, string userId)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress percentage must be between 0 and 100");

        ProgressPercentage = percentage;
        ActualCost = actualCost;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;

        // Auto-update status based on progress
        if (percentage == 100 && Status == PhaseStatus.InProgress)
        {
            Status = PhaseStatus.Completed;
            ActualEndDate = DateTime.UtcNow;
        }
    }

    public bool IsOverBudget() => ActualCost > ApprovedBudget;
    public bool IsDelayed() => ActualEndDate > PlannedEndDate || DateTime.UtcNow > PlannedEndDate && Status != PhaseStatus.Completed;
    public decimal BudgetVariance() => ApprovedBudget - ActualCost;
    public decimal BudgetVariancePercentage() => ApprovedBudget > 0 ? BudgetVariance() / ApprovedBudget * 100 : 0;
}