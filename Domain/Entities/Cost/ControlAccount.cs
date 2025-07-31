using Core.Enums.Cost;
using Domain.Common;
using Domain.Entities.EVM;
using Domain.Entities.Projects;
using Domain.Entities.Security;
using Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Cost;

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
    public ICollection<PlanningPackage> PlanningPackages { get; private set; } = new List<PlanningPackage>();
    public ICollection<EVMRecord> EVMRecords { get; private set; } = new List<EVMRecord>();
    public ICollection<ControlAccountAssignment> Assignments { get; private set; } = new List<ControlAccountAssignment>();

    // Constructor for EF Core
    private ControlAccount() { }

    public ControlAccount(
        string code,
        string name,
        Guid projectId,
        Guid phaseId,
        Guid camUserId,
        decimal bac,
        MeasurementMethod measurementMethod)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProjectId = projectId;
        PhaseId = phaseId;
        CAMUserId = camUserId;
        BAC = bac;
        MeasurementMethod = measurementMethod;

        Status = ControlAccountStatus.Planning;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        BaselineDate = DateTime.UtcNow;
        PercentComplete = 0;
        ContingencyReserve = 0;
        ManagementReserve = 0;

        Validate();
    }

    // Methods
    public void UpdateBasicInfo(string name, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBudget(decimal bac, decimal contingencyReserve, decimal managementReserve)
    {
        if (Status == ControlAccountStatus.Closed)
            throw new InvalidOperationException("Cannot update budget for closed control account");

        BAC = bac;
        ContingencyReserve = contingencyReserve;
        ManagementReserve = managementReserve;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateCAM(Guid camUserId)
    {
        CAMUserId = camUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMeasurementMethod(MeasurementMethod method)
    {
        if (Status == ControlAccountStatus.InProgress)
            throw new InvalidOperationException("Cannot change measurement method for in-progress control account");

        MeasurementMethod = method;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ControlAccountStatus newStatus)
    {
        if (!IsValidStatusTransition(Status, newStatus))
            throw new InvalidOperationException($"Invalid status transition from {Status} to {newStatus}");

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal percentComplete)
    {
        if (percentComplete < 0 || percentComplete > 100)
            throw new ArgumentOutOfRangeException(nameof(percentComplete), "Progress must be between 0 and 100");

        PercentComplete = percentComplete;
        UpdatedAt = DateTime.UtcNow;

        // Auto-update status based on progress
        if (percentComplete == 100 && Status == ControlAccountStatus.InProgress)
        {
            Status = ControlAccountStatus.Completed;
        }
    }

    public void Baseline()
    {
        if (Status != ControlAccountStatus.Planning)
            throw new InvalidOperationException("Only planning control accounts can be baselined");

        Status = ControlAccountStatus.Baselined;
        BaselineDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status != ControlAccountStatus.Completed)
            throw new InvalidOperationException("Only completed control accounts can be closed");

        Status = ControlAccountStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }

    // New helper methods for WBSElement
    public IEnumerable<WBSElement> GetActiveWorkPackages()
    {
        return WorkPackages.Where(w => w.IsWorkPackage() && w.IsActive && !w.IsDeleted);
    }

    public IEnumerable<WBSElement> GetPlanningPackages()
    {
        return WorkPackages.Where(w => w.IsPlanningPackage() && w.IsActive && !w.IsDeleted);
    }

    public void AddWorkPackage(WBSElement wbsElement)
    {
        if (!wbsElement.IsWorkPackage() && !wbsElement.IsPlanningPackage())
            throw new InvalidOperationException("Only work packages or planning packages can be added to control account");

        if (wbsElement.ControlAccountId != Id)
            throw new InvalidOperationException("Work package must be assigned to this control account");

        WorkPackages.Add(wbsElement);
        UpdatedAt = DateTime.UtcNow;
    }

    // Calculate rollup from work packages
    public void CalculateRollupProgress()
    {
        var activeWorkPackages = GetActiveWorkPackages().ToList();
        if (!activeWorkPackages.Any())
        {
            PercentComplete = 0;
            return;
        }

        var totalBudget = activeWorkPackages.Sum(wp => wp.WorkPackageDetails?.Budget ?? 0);
        if (totalBudget == 0)
        {
            // Simple average if no budget
            PercentComplete = activeWorkPackages.Average(wp => wp.WorkPackageDetails?.ProgressPercentage ?? 0);
        }
        else
        {
            // Weighted average by budget
            var weightedProgress = activeWorkPackages.Sum(wp =>
                (wp.WorkPackageDetails?.ProgressPercentage ?? 0) * (wp.WorkPackageDetails?.Budget ?? 0));
            PercentComplete = weightedProgress / totalBudget;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    // Validation
    private void Validate()
    {
        if (BAC < 0)
            throw new ArgumentException("Budget at Completion cannot be negative");

        if (ContingencyReserve < 0)
            throw new ArgumentException("Contingency Reserve cannot be negative");

        if (ManagementReserve < 0)
            throw new ArgumentException("Management Reserve cannot be negative");

        if (!System.Text.RegularExpressions.Regex.IsMatch(Code, @"^C-\d{3}-\d{2}-[A-Z]{3}-\d{2}$"))
            throw new ArgumentException("Code must follow format: C-XXX-YY-CAM-##");
    }

    private bool IsValidStatusTransition(ControlAccountStatus current, ControlAccountStatus next)
    {
        return (current, next) switch
        {
            (ControlAccountStatus.Planning, ControlAccountStatus.Baselined) => true,
            (ControlAccountStatus.Baselined, ControlAccountStatus.InProgress) => true,
            (ControlAccountStatus.InProgress, ControlAccountStatus.Completed) => true,
            (ControlAccountStatus.Completed, ControlAccountStatus.Closed) => true,
            (ControlAccountStatus.InProgress, ControlAccountStatus.OnHold) => true,
            (ControlAccountStatus.OnHold, ControlAccountStatus.InProgress) => true,
            _ => false
        };
    }
}