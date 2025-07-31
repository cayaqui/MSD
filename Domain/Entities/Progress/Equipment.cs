using Domain.Entities.Projects;

namespace Domain.Entities.Progress;

/// <summary>
/// Equipment entity for tracking equipment resources
/// </summary>
public class Equipment : BaseEntity, IAuditable, ISoftDelete, IActivatable
{
    // Basic Information
    public string EquipmentCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Model { get; private set; }
    public string? SerialNumber { get; private set; }

    // Classification
    public string Category { get; private set; } = string.Empty; // Crane, Excavator, etc.
    public string? Manufacturer { get; private set; }
    public int? YearOfManufacture { get; private set; }

    // Ownership
    public bool IsOwned { get; private set; }
    public Guid? ContractorId { get; private set; } // If rented
    public decimal? PurchaseValue { get; private set; }
    public DateTime? PurchaseDate { get; private set; }

    // Rates
    public decimal? DailyRate { get; private set; }
    public decimal? HourlyRate { get; private set; }
    public string Currency { get; private set; } = "USD";

    // Status
    public bool IsActive { get; set; }
    public bool IsAvailable { get; private set; }
    public string? CurrentLocation { get; private set; }
    public Guid? CurrentProjectId { get; private set; }

    // Maintenance
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public decimal? MaintenanceHours { get; private set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Contractor? Contractor { get; private set; }
    public Project? CurrentProject { get; private set; }
    public ICollection<Resource> ResourceAllocations { get; private set; } = new List<Resource>();

    // Constructor for EF Core
    private Equipment() { }

    public Equipment(
        string equipmentCode,
        string name,
        string category,
        bool isOwned)
    {
        EquipmentCode = equipmentCode ?? throw new ArgumentNullException(nameof(equipmentCode));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        IsOwned = isOwned;

        IsActive = true;
        IsAvailable = true;
        CreatedAt = DateTime.UtcNow;
    }

    // Methods
    public void UpdateDetails(string name, string? description, string? model, string? manufacturer)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Model = model;
        Manufacturer = manufacturer;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRates(decimal? dailyRate, decimal? hourlyRate, string currency)
    {
        DailyRate = dailyRate;
        HourlyRate = hourlyRate;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        UpdatedAt = DateTime.UtcNow;

        if (dailyRate.HasValue && dailyRate.Value < 0)
            throw new ArgumentException("Daily rate cannot be negative");

        if (hourlyRate.HasValue && hourlyRate.Value < 0)
            throw new ArgumentException("Hourly rate cannot be negative");
    }

    public void AssignToProject(Guid projectId, string location)
    {
        CurrentProjectId = projectId;
        CurrentLocation = location;
        IsAvailable = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseFromProject()
    {
        CurrentProjectId = null;
        IsAvailable = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordMaintenance(DateTime maintenanceDate, decimal hours, DateTime? nextMaintenanceDate = null)
    {
        LastMaintenanceDate = maintenanceDate;
        MaintenanceHours = (MaintenanceHours ?? 0) + hours;
        if (nextMaintenanceDate.HasValue)
        {
            NextMaintenanceDate = nextMaintenanceDate.Value;
        }
        UpdatedAt = DateTime.UtcNow;
    }
}