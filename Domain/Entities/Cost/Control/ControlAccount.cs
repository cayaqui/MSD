using Domain.Entities.Cost.Core;
using Domain.Entities.Cost.EVM;
using Domain.Entities.WBS;

namespace Domain.Entities.Cost.Control;

/// <summary>
/// Control Account entity based on PMI standards
/// Format: C-XXX-YY-CAM-##
/// </summary>
public class ControlAccount : BaseEntity, IAuditable, ISoftDelete, IActivatable
{
    // Basic Information
    public string Code { get; private set; } = string.Empty; // C-XXX-YY-CAM-##
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Foreign Keys
    public Guid ProjectId { get; private set; }
    public Guid PhaseId { get; private set; }
    public Guid CAMUserId { get; private set; } // Control Account Manager

    // Budget Information
    public decimal BAC { get; private set; } // Budget at Completion
    public decimal ContingencyReserve { get; private set; }
    public decimal ManagementReserve { get; private set; }

    // Control Information
    public MeasurementMethod MeasurementMethod { get; private set; }
    public ControlAccountStatus Status { get; private set; }
    public DateTime BaselineDate { get; private set; }

    // Calculated Fields
    public decimal TotalBudget => BAC + ContingencyReserve + ManagementReserve;
    public decimal PercentComplete { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Activatable
    public bool IsActive { get; set; }

    // Navigation Properties
    public Project Project { get; private set; } = null!;
    public Phase Phase { get; private set; } = null!;
    public User CAMUser { get; private set; } = null!;
    public ICollection<WBSElement> WorkPackages { get; private set; } = new List<WBSElement>(); // Updated
    public ICollection<PlanningPackage> PlanningPackages { get; private set; } =
        new List<PlanningPackage>();
    public ICollection<EVMRecord> EVMRecords { get; private set; } = new List<EVMRecord>();
    public ICollection<ControlAccountAssignment> Assignments { get; private set; } =
        new List<ControlAccountAssignment>();
    public ICollection<CostControlReport> CostControlReports { get; private set; } =
        new List<CostControlReport>();
    public ICollection<TimePhasedBudget> TimePhasedBudgets { get; private set; } =
        new List<TimePhasedBudget>();
    public ICollection<CostItem> CostItems { get; private set; } = new List<CostItem>();
    public ICollection<ActualCost> ActualCosts { get; private set; } = new List<ActualCost>();

    // Constructor for EF Core
    private ControlAccount() { }

    public ControlAccount(
        string code,
        string name,
        Guid projectId,
        Guid phaseId,
        Guid camUserId,
        decimal bac,
        MeasurementMethod measurementMethod
    )
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProjectId = projectId;
        PhaseId = phaseId;
        CAMUserId = camUserId;
        BAC = bac;
        MeasurementMethod = measurementMethod;
        Status = ControlAccountStatus.Open;
        BaselineDate = DateTime.UtcNow;
        IsActive = true;
    }

    // Domain Methods
    public void UpdateBasicInfo(string name, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBudget(decimal bac, decimal contingencyReserve, decimal managementReserve)
    {
        if (Status == ControlAccountStatus.Closed)
            throw new InvalidOperationException("Cannot update budget on a closed Control Account");

        if (bac < 0 || contingencyReserve < 0 || managementReserve < 0)
            throw new ArgumentException("Budget values cannot be negative");

        BAC = bac;
        ContingencyReserve = contingencyReserve;
        ManagementReserve = managementReserve;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignCAM(Guid newCAMUserId, string updatedBy)
    {
        if (Status == ControlAccountStatus.Closed)
            throw new InvalidOperationException("Cannot change CAM on a closed Control Account");

        CAMUserId = newCAMUserId;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ControlAccountStatus newStatus, string updatedBy)
    {
        // Validate status transition
        switch (Status)
        {
            case ControlAccountStatus.Open:
                if (
                    newStatus != ControlAccountStatus.Baselined
                    && newStatus != ControlAccountStatus.Closed
                )
                    throw new InvalidOperationException(
                        $"Cannot transition from {Status} to {newStatus}"
                    );
                break;
            case ControlAccountStatus.Baselined:
                if (
                    newStatus != ControlAccountStatus.InProgress
                    && newStatus != ControlAccountStatus.Closed
                )
                    throw new InvalidOperationException(
                        $"Cannot transition from {Status} to {newStatus}"
                    );
                break;
            case ControlAccountStatus.InProgress:
                if (
                    newStatus != ControlAccountStatus.Completed
                    && newStatus != ControlAccountStatus.Closed
                )
                    throw new InvalidOperationException(
                        $"Cannot transition from {Status} to {newStatus}"
                    );
                break;
            case ControlAccountStatus.Completed:
                if (newStatus != ControlAccountStatus.Closed)
                    throw new InvalidOperationException(
                        $"Cannot transition from {Status} to {newStatus}"
                    );
                break;
            case ControlAccountStatus.Closed:
                throw new InvalidOperationException(
                    "Cannot change status of a closed Control Account"
                );
        }

        Status = newStatus;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == ControlAccountStatus.Baselined)
        {
            BaselineDate = DateTime.UtcNow;
        }
    }

    public void UpdateProgress(decimal percentComplete, string updatedBy)
    {
        if (percentComplete < 0 || percentComplete > 100)
            throw new ArgumentException("Percent complete must be between 0 and 100");

        if (Status == ControlAccountStatus.Closed)
            throw new InvalidOperationException(
                "Cannot update progress on a closed Control Account"
            );

        PercentComplete = percentComplete;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;

        // Auto-update status based on progress
        if (percentComplete == 100 && Status == ControlAccountStatus.InProgress)
        {
            Status = ControlAccountStatus.Completed;
        }
        else if (
            percentComplete > 0
            && percentComplete < 100
            && Status == ControlAccountStatus.Baselined
        )
        {
            Status = ControlAccountStatus.InProgress;
        }
    }

    public void Close(string closedBy)
    {
        if (Status == ControlAccountStatus.Closed)
            throw new InvalidOperationException("Control Account is already closed");

        // Verify all work packages are completeg
        var incompleteWorkPackages = WorkPackages
            .Where(wp => wp.GetWeightedProgress() < 100)
            .ToList();
        if (incompleteWorkPackages.Any())
            throw new InvalidOperationException(
                $"Cannot close Control Account with {incompleteWorkPackages.Count} incomplete work packages"
            );

        Status = ControlAccountStatus.Closed;
        UpdatedBy = closedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete(string deletedBy)
    {
        if (Status != ControlAccountStatus.Open)
            throw new InvalidOperationException(
                "Only Control Accounts in Open status can be deleted"
            );

        if (WorkPackages.Any() || PlanningPackages.Any())
            throw new InvalidOperationException(
                "Cannot delete Control Account with assigned work packages or planning packages"
            );

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    // Calculations
    public decimal GetActualCost()
    {
        return EVMRecords.OrderByDescending(e => e.DataDate).FirstOrDefault()?.AC ?? 0;
    }

    public decimal GetEarnedValue()
    {
        return EVMRecords.OrderByDescending(e => e.DataDate).FirstOrDefault()?.EV ?? 0;
    }

    public decimal GetPlannedValue()
    {
        return EVMRecords.OrderByDescending(e => e.DataDate).FirstOrDefault()?.PV ?? 0;
    }

    public decimal GetCPI()
    {
        var ev = GetEarnedValue();
        var ac = GetActualCost();
        return ac > 0 ? ev / ac : 0;
    }

    public decimal GetSPI()
    {
        var ev = GetEarnedValue();
        var pv = GetPlannedValue();
        return pv > 0 ? ev / pv : 0;
    }
}
