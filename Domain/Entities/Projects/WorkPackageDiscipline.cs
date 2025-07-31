using System;
using Domain.Common;
using Domain.Entities.Setup;
using Domain.Entities.Security;

namespace Domain.Entities.Projects;

/// <summary>
/// Representa la participación de una disciplina en un Work Package
/// </summary>
public class WorkPackageDiscipline : BaseEntity
{
    // Foreign Keys
    public Guid WorkPackageDetailsId { get; private set; }
    public Guid DisciplineId { get; private set; }

    // Work allocation
    public decimal EstimatedHours { get; private set; }
    public decimal ActualHours { get; private set; }
    public bool IsPrimaryDiscipline { get; private set; }

    // Resource assignment
    public Guid? LeadEngineerId { get; private set; }
    public string? Notes { get; private set; }

    // Performance
    public decimal ProgressPercentage { get; private set; }
    public decimal ProductivityRate { get; private set; } // Actual/Estimated

    // Cost allocation
    public decimal EstimatedCost { get; private set; }
    public decimal ActualCost { get; private set; }

    // Navigation properties
    public WorkPackageDetails WorkPackageDetails { get; private set; } = null!;
    public Discipline Discipline { get; private set; } = null!;
    public User? LeadEngineer { get; private set; }

    private WorkPackageDiscipline() { } // EF Core

    public WorkPackageDiscipline(
        Guid workPackageDetailsId,
        Guid disciplineId,
        decimal estimatedHours,
        bool isPrimary = false)
    {
        WorkPackageDetailsId = workPackageDetailsId;
        DisciplineId = disciplineId;
        EstimatedHours = estimatedHours;
        IsPrimaryDiscipline = isPrimary;
        ActualHours = 0;
        ProgressPercentage = 0;
        ProductivityRate = 0;
        EstimatedCost = 0;
        ActualCost = 0;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateEstimate(decimal estimatedHours, decimal? estimatedCost = null)
    {
        if (estimatedHours < 0)
            throw new ArgumentException("Estimated hours cannot be negative");

        EstimatedHours = estimatedHours;

        if (estimatedCost.HasValue)
        {
            if (estimatedCost.Value < 0)
                throw new ArgumentException("Estimated cost cannot be negative");
            EstimatedCost = estimatedCost.Value;
        }

        UpdateProductivityRate();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateActuals(decimal actualHours, decimal progressPercentage, decimal? actualCost = null)
    {
        if (actualHours < 0)
            throw new ArgumentException("Actual hours cannot be negative");

        if (progressPercentage < 0 || progressPercentage > 100)
            throw new ArgumentException("Progress must be between 0 and 100");

        ActualHours = actualHours;
        ProgressPercentage = progressPercentage;

        if (actualCost.HasValue)
        {
            if (actualCost.Value < 0)
                throw new ArgumentException("Actual cost cannot be negative");
            ActualCost = actualCost.Value;
        }

        UpdateProductivityRate();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignLeadEngineer(Guid engineerId)
    {
        LeadEngineerId = engineerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveLeadEngineer()
    {
        LeadEngineerId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsPrimary()
    {
        IsPrimaryDiscipline = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdateProductivityRate()
    {
        if (EstimatedHours > 0 && ProgressPercentage > 0)
        {
            var expectedHours = EstimatedHours * (ProgressPercentage / 100);
            ProductivityRate = expectedHours > 0 ? ActualHours / expectedHours : 0;
        }
        else
        {
            ProductivityRate = 0;
        }
    }

    private void Validate()
    {
        if (EstimatedHours < 0)
            throw new ArgumentException("Estimated hours cannot be negative");
    }

    // Calculated properties
    public decimal GetEfficiency() => EstimatedHours > 0 ? (EstimatedHours - ActualHours) / EstimatedHours * 100 : 0;

    public decimal GetCostVariance() => EstimatedCost - ActualCost;

    public decimal GetCostPerformanceIndex() => ActualCost > 0 ? (EstimatedCost * (ProgressPercentage / 100)) / ActualCost : 0;

    public bool IsOverrunning() => ActualHours > EstimatedHours || ActualCost > EstimatedCost;
}