namespace Domain.Entities.Cost;

/// <summary>
/// Control Account Assignment for team members
/// </summary>
public class ControlAccountAssignment : BaseEntity
{
    public Guid ControlAccountId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public ControlAccountRole Role { get; private set; }
    public DateTime AssignedDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? AllocationPercentage { get; private set; }

    // Navigation Properties
    public ControlAccount ControlAccount { get; private set; } = null!;
    public User User { get; private set; } = null!;

    // Constructor for EF Core
    private ControlAccountAssignment() { }

    public ControlAccountAssignment(
        Guid controlAccountId,
        string userId,
        ControlAccountRole role,
        decimal? allocationPercentage = null)
    {
        ControlAccountId = controlAccountId;
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Role = role;
        AllocationPercentage = allocationPercentage;

        AssignedDate = DateTime.UtcNow;
        IsActive = true;

        Validate();
    }

    public void EndAssignment()
    {
        IsActive = false;
        EndDate = DateTime.UtcNow;
    }

    public void UpdateAllocation(decimal percentage)
    {
        AllocationPercentage = percentage;
        Validate();
    }

    private void Validate()
    {
        if (AllocationPercentage.HasValue && (AllocationPercentage < 0 || AllocationPercentage > 100))
            throw new ArgumentException("Allocation percentage must be between 0 and 100");
    }
}
