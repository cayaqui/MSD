namespace Domain.Entities.Projects;

public class Phase : BaseEntity, ISoftDelete, ICodeEntity, INamedEntity, IDescribable, IOrderable
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }

    // Dates
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }

    // Status
    public PhaseStatus Status { get; private set; } = PhaseStatus.NotStarted;

    // Progress
    public decimal ProgressPercentage { get; private set; } = 0;

    // Foreign Keys
    public Guid ProjectId { get; private set; }

    // Navigation properties
    public Project Project { get; private set; } = null!;

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    private Phase() { } // EF Core

    public Phase(
        Guid projectId,
        string name,
        int order,
        DateTime plannedStartDate,
        DateTime plannedEndDate
    )
    {
        ProjectId = projectId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Order = order;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;

        if (plannedEndDate < plannedStartDate)
            throw new ArgumentException("End date cannot be before start date");
    }

    public void UpdateBasicInfo(string name, string? description, int order)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Order = order;
    }

    public void UpdatePlannedDates(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be before start date");

        PlannedStartDate = startDate;
        PlannedEndDate = endDate;
    }

    public void UpdateActualDates(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
            throw new ArgumentException("End date cannot be before start date");

        ActualStartDate = startDate;
        ActualEndDate = endDate;
    }

    public void UpdateProgress(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress must be between 0 and 100");

        ProgressPercentage = percentage;
    }

    public void UpdateStatus(PhaseStatus status)
    {
        Status = status;
    }

    public void Start()
    {
        if (!Status.CanStart())
            throw new InvalidOperationException($"Phase cannot be started from {Status} status");

        Status = PhaseStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (!Status.IsActive())
            throw new InvalidOperationException($"Phase must be active to complete");

        Status = PhaseStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        ProgressPercentage = 100;
    }
}
