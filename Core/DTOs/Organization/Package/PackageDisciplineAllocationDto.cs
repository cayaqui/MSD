namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for package discipline allocation summary
/// </summary>
public class PackageDisciplineAllocationDto
{
    public Guid PackageId { get; set; }
    public int TotalDisciplines { get; set; }
    public decimal TotalEstimatedManHours { get; set; }
    public decimal TotalActualManHours { get; set; }
    public decimal TotalEstimatedCost { get; set; }
    public decimal TotalActualCost { get; set; }
    public decimal AverageProgress { get; set; }
    public List<DisciplineAllocationBreakdownDto> DisciplineBreakdown { get; set; } = new();
}

/// <summary>
/// DTO for individual discipline allocation
/// </summary>
public class DisciplineAllocationDto
{
    public Guid DisciplineId { get; set; }
    public string DisciplineCode { get; set; } = string.Empty;
    public string DisciplineName { get; set; } = string.Empty;
    public bool IsLeadDiscipline { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal ProgressPercentage { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

/// <summary>
/// DTO for discipline allocation breakdown
/// </summary>
public class DisciplineAllocationBreakdownDto
{
    public Guid DisciplineId { get; set; }
    public string DisciplineCode { get; set; } = string.Empty;
    public string DisciplineName { get; set; } = string.Empty;
    public bool IsLead { get; set; }
    public decimal EstimatedManHours { get; set; }
    public decimal ActualManHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal ProgressPercentage { get; set; }
    public decimal ManHoursPercentage { get; set; }
    public decimal CostPercentage { get; set; }
}