namespace Domain.Entities.Projects;

public class Package : BaseEntity, ISoftDelete, ICodeEntity, INamedEntity, IDescribable
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // WBS
    public string WBSCode { get; private set; } = string.Empty;

    // Type
    public PackageType PackageType { get; private set; }

    // Contract Information
    public string? ContractNumber { get; private set; }
    public string? ContractType { get; private set; } // Lump Sum, Unit Price, Cost Plus, etc.
    public decimal ContractValue { get; private set; }
    public string Currency { get; private set; } = "USD";

    // Dates
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }

    // Progress
    public decimal ProgressPercentage { get; private set; } = 0;

    // Foreign Keys
    public Guid? PhaseId { get; private set; }
    public Guid? WBSElementId { get; private set; }
    public Guid? ContractorId { get; private set; }

    // Navigation properties
    public Phase? Phase { get; private set; }
    public WBSElement? WBSElement { get; private set; }
    public Contractor? Contractor { get; private set; }
    public ICollection<PackageDiscipline> PackageDisciplines { get; private set; } =
        new List<PackageDiscipline>();
    public ICollection<BudgetItem> BudgetItems { get; private set; } =
        new List<BudgetItem>();

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    private Package() { } // EF Core

    public Package(
        string code,
        string name,
        string wbsCode,
        PackageType packageType,
        DateTime plannedStartDate,
        DateTime plannedEndDate
    )
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        WBSCode = wbsCode ?? throw new ArgumentNullException(nameof(wbsCode));
        PackageType = packageType;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;

        if (plannedEndDate < plannedStartDate)
            throw new ArgumentException("End date cannot be before start date");
    }

    public void AssignToPhase(Guid phaseId)
    {
        if (WBSElementId.HasValue)
            throw new InvalidOperationException(
                "Package cannot be assigned to both Phase and WBSElement"
            );

        PhaseId = phaseId;
        WBSElementId = null;
    }

    public void AssignToWorkPackage(Guid wbsElementId)
    {
        if (PhaseId.HasValue)
            throw new InvalidOperationException(
                "Package cannot be assigned to both Phase and WorkPackage"
            );

        WBSElementId = wbsElementId;
        PhaseId = null;
    }

    public void AssignContractor(Guid? contractorId)
    {
        ContractorId = contractorId;
    }

    public void UpdateContractInfo(
        string? contractNumber,
        string? contractType,
        decimal contractValue,
        string currency
    )
    {
        ContractNumber = contractNumber;
        ContractType = contractType;
        ContractValue = contractValue;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public void UpdateProgress(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Progress must be between 0 and 100");

        ProgressPercentage = percentage;
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

    public void UpdateBasicInfo(string name, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
    }

    public void UpdatePackageType(PackageType packageType)
    {
        PackageType = packageType;
    }

    public void UpdateWBSCode(string wbsCode)
    {
        WBSCode = wbsCode ?? throw new ArgumentNullException(nameof(wbsCode));
    }

    public void Start()
    {
        if (ActualStartDate.HasValue)
            throw new InvalidOperationException("Package has already started");

        ActualStartDate = DateTime.UtcNow;

        if (ProgressPercentage == 0)
            ProgressPercentage = 1; // Set minimum progress when started
    }

    public void Complete()
    {
        if (!ActualStartDate.HasValue)
            throw new InvalidOperationException("Package must be started before completing");

        ActualEndDate = DateTime.UtcNow;
        ProgressPercentage = 100;
    }

    public void Cancel()
    {
        if (ProgressPercentage == 100)
            throw new InvalidOperationException("Cannot cancel a completed package");

        if (!ActualStartDate.HasValue)
        {
            // If not started, just mark as cancelled
            ActualStartDate = DateTime.UtcNow;
        }

        ActualEndDate = DateTime.UtcNow;
        // Progress remains at current percentage
    }

    public bool IsOverdue()
    {
        if (ActualEndDate.HasValue || ProgressPercentage == 100)
            return false;

        return DateTime.UtcNow > PlannedEndDate;
    }

    public int GetDaysOverdue()
    {
        if (!IsOverdue())
            return 0;

        return (DateTime.UtcNow - PlannedEndDate).Days;
    }

    public decimal GetScheduleVariance()
    {
        if (!ActualStartDate.HasValue)
            return 0;

        var plannedDuration = (PlannedEndDate - PlannedStartDate).Days;
        var actualDuration = ActualEndDate.HasValue
            ? (ActualEndDate.Value - ActualStartDate.Value).Days
            : (DateTime.UtcNow - ActualStartDate.Value).Days;

        return (decimal)(actualDuration - plannedDuration) / plannedDuration * 100;
    }

    public void AddDiscipline(PackageDiscipline packageDiscipline)
    {
        if (packageDiscipline == null)
            throw new ArgumentNullException(nameof(packageDiscipline));

        if (PackageDisciplines.Any(pd => pd.DisciplineId == packageDiscipline.DisciplineId))
            throw new InvalidOperationException("Discipline already assigned to this package");

        PackageDisciplines.Add(packageDiscipline);
    }

    public void RemoveDiscipline(Guid disciplineId)
    {
        var packageDiscipline = PackageDisciplines.FirstOrDefault(pd =>
            pd.DisciplineId == disciplineId
        );
        if (packageDiscipline != null)
        {
            PackageDisciplines.Remove(packageDiscipline);
        }
    }

    public decimal GetTotalEstimatedHours()
    {
        return PackageDisciplines.Sum(pd => pd.EstimatedHours);
    }

    public decimal GetTotalActualHours()
    {
        return PackageDisciplines.Sum(pd => pd.ActualHours);
    }

    public decimal GetHoursVariance()
    {
        var estimated = GetTotalEstimatedHours();
        var actual = GetTotalActualHours();

        if (estimated == 0)
            return 0;

        return (actual - estimated) / estimated * 100;
    }
}
