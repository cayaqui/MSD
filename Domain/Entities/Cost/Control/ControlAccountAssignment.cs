
using Domain.Entities.Auth.Security;

namespace Domain.Entities.Cost.Control;

/// <summary>
/// Assignment of team members to Control Accounts
/// Tracks roles and responsibilities within a Control Account
/// </summary>
public class ControlAccountAssignment : BaseEntity, IAuditable, IActivatable
{
    // Foreign Keys
    public Guid ControlAccountId { get; private set; }
    public Guid UserId { get; private set; }

    // Assignment Information
    public ControlAccountRole Role { get; private set; }
    public DateTime AssignedDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public decimal? AllocationPercentage { get; private set; } // % of time allocated
    public string? Notes { get; private set; }

    // Activatable
    public bool IsActive { get; set; }

    // Navigation Properties
    public ControlAccount ControlAccount { get; private set; } = null!;
    public User User { get; private set; } = null!;

    // Constructor for EF Core
    private ControlAccountAssignment() { }

    public ControlAccountAssignment(
        Guid controlAccountId,
        Guid userId,
        ControlAccountRole role,
        decimal? allocationPercentage = null)
    {
        ControlAccountId = controlAccountId;
        UserId = userId;
        Role = role;
        AllocationPercentage = allocationPercentage;
        AssignedDate = DateTime.UtcNow;
        IsActive = true;
    }

    // Domain Methods
    public void UpdateRole(ControlAccountRole newRole, string updatedBy)
    {
        Role = newRole;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAllocation(decimal percentage, string updatedBy)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Allocation percentage must be between 0 and 100");

        AllocationPercentage = percentage;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EndAssignment(string endedBy)
    {
        if (EndDate.HasValue)
            throw new InvalidOperationException("Assignment has already ended");

        EndDate = DateTime.UtcNow;
        IsActive = false;
        UpdatedBy = endedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddNotes(string notes, string updatedBy)
    {
        Notes = notes;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsCurrentlyActive()
    {
        return IsActive && !EndDate.HasValue;
    }
}