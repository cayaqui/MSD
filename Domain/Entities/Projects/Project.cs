namespace Domain.Entities.Projects;

public class Project : BaseEntity, IAuditable, ISoftDelete, IActivatable
{
    public string WBSCode { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Foreign Keys
    public Guid OperationId { get; private set; }
    public Operation Operation { get; private set; } = null!;

    // Project Details
    public string? ProjectCharter { get; private set; }
    public string? Objectives { get; private set; }
    public string? Scope { get; private set; }
    public string? Deliverables { get; private set; }

    public ProjectStatus Status { get; private set; }
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }

    // Financial
    public decimal TotalBudget { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal? ActualCost { get; private set; }

    // Additional Info
    public string? ProjectManagerId { get; set; }
    public string? Location { get; private set; }
    public string? Client { get; private set; }
    public string? ContractNumber { get; private set; }

    // Calculated Fields
    public decimal ProgressPercentage { get; private set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // IActivatable
    public bool IsActive { get; set; }

    // Navigation Properties
    //public ICollection<Phase> Phases { get; private set; } = new List<Phase>();
    //public ICollection<WorkPackage> WorkPackages { get; private set; } = new List<WorkPackage>();
    //public ICollection<Package> Packages { get; private set; } = new List<Package>();
    public ICollection<ProjectTeamMember> ProjectTeamMembers { get; private set; } =
        new List<ProjectTeamMember>();
    //public ICollection<Budget> BudgetVersions { get; private set; } = new List<Budget>();
    //public ICollection<ScheduleVersion> ScheduleVersions { get; private set; } =
    //    new List<ScheduleVersion>();
    //public ICollection<Contingency> Contingencies { get; private set; } = new List<Contingency>();
    //public ICollection<Trend> Trends { get; private set; } = new List<Trend>();
    //public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();

    private Project() { } // EF Core

    public Project(
        string code,
        string name,
        string description,
        Guid operationId,
        DateTime plannedStartDate,
        DateTime plannedEndDate,
        decimal budget,
        string currency,
        string location
    )
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        OperationId = operationId;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        TotalBudget = budget;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));

        Status = ProjectStatus.Planning;
        IsActive = true;
        ProgressPercentage = 0;
        CreatedAt = DateTime.UtcNow;

        ValidateDates();
    }

    public void UpdateDetails(
        string name,
        string? description,
        string? location,
        string? client,
        string? contractNumber,
        string? currency
    )
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Location = location;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        Client = client;
        ContractNumber = contractNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePlannedDates(DateTime startDate, DateTime endDate)
    {
        PlannedStartDate = startDate;
        PlannedEndDate = endDate;
        ValidateDates();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActualStartDate(DateTime startDate)
    {
        ActualStartDate = startDate;
        if (Status == ProjectStatus.Planning)
        {
            Status = ProjectStatus.Active;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActualEndDate(DateTime endDate)
    {
        ActualEndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBudget(decimal budget)
    {
        if (budget < 0)
        {
            throw new ArgumentException("BudgetItem cannot be negative", nameof(budget));
        }
        TotalBudget = budget;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ProjectStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateWbsProfile(string wbs, string? client = null, string? contractNumber = null)
    {
        if (string.IsNullOrWhiteSpace(wbs))
        {
            throw new ArgumentException("WBS code cannot be null or empty", nameof(wbs));
        }
        WBSCode = wbs;
        Client = client;
        ContractNumber = contractNumber;
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateProgress(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            throw new ArgumentException(
                "Progress percentage must be between 0 and 100",
                nameof(percentage)
            );
        }
        ProgressPercentage = percentage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidateDates()
    {
        if (PlannedEndDate <= PlannedStartDate)
        {
            throw new InvalidOperationException(
                "Planned end date must be after planned start date"
            );
        }
    }
}
