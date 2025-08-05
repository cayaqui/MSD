using Domain.Entities.Auth.Security;

namespace Domain.Entities.Organization.Core;

/// <summary>
/// Represents the participation of a discipline in a Package
/// </summary>
public class PackageDiscipline : BaseEntity
{
    // Foreign Keys
    public Guid PackageId { get; private set; }
    public Guid DisciplineId { get; private set; }

    // Work allocation
    public decimal EstimatedHours { get; private set; }
    public decimal ActualHours { get; private set; }
    public bool IsLeadDiscipline { get; private set; }

    // Resource assignment
    public Guid? LeadEngineerId { get; private set; }
    public string? Notes { get; private set; }

    // Progress
    public decimal ProgressPercentage { get; private set; }
    public DateTime? LastProgressUpdate { get; private set; }

    // Cost allocation
    public decimal EstimatedCost { get; private set; }
    public decimal ActualCost { get; private set; }
    public string Currency { get; private set; } = "USD";

    // Calculated fields
    public decimal ProductivityRate => EstimatedHours > 0 ? ActualHours / EstimatedHours : 0;
    public decimal CostVariance => EstimatedCost - ActualCost;
    public decimal HoursVariance => EstimatedHours - ActualHours;

    // Navigation properties
    public Package Package { get; private set; } = null!;
    public Discipline Discipline { get; private set; } = null!;
    public User? LeadEngineer { get; private set; }

    private PackageDiscipline() { } // EF Core

    public PackageDiscipline(
        Guid packageId,
        Guid disciplineId,
        decimal estimatedHours,
        decimal estimatedCost,
        bool isLeadDiscipline = false)
    {
        PackageId = packageId;
        DisciplineId = disciplineId;
        EstimatedHours = estimatedHours;
        EstimatedCost = estimatedCost;
        IsLeadDiscipline = isLeadDiscipline;

        ActualHours = 0;
        ActualCost = 0;
        ProgressPercentage = 0;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateEstimate(decimal estimatedHours, decimal estimatedCost)
    {
        if (estimatedHours < 0)
            throw new ArgumentException("Estimated hours cannot be negative");
        if (estimatedCost < 0)
            throw new ArgumentException("Estimated cost cannot be negative");

        EstimatedHours = estimatedHours;
        EstimatedCost = estimatedCost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateActuals(decimal actualHours, decimal actualCost, decimal progressPercentage)
    {
        if (actualHours < 0)
            throw new ArgumentException("Actual hours cannot be negative");
        if (actualCost < 0)
            throw new ArgumentException("Actual cost cannot be negative");
        if (progressPercentage < 0 || progressPercentage > 100)
            throw new ArgumentException("Progress percentage must be between 0 and 100");

        ActualHours = actualHours;
        ActualCost = actualCost;
        ProgressPercentage = progressPercentage;
        LastProgressUpdate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignLeadEngineer(Guid? leadEngineerId)
    {
        LeadEngineerId = leadEngineerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsLeadDiscipline(bool isLead)
    {
        IsLeadDiscipline = isLead;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (EstimatedHours < 0)
            throw new ArgumentException("Estimated hours cannot be negative");
        if (EstimatedCost < 0)
            throw new ArgumentException("Estimated cost cannot be negative");
    }
}