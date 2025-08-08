namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// Detailed DTO for planning package with all related information
/// </summary>
public class PlanningPackageDetailDto : PlanningPackageDto
{
    public List<PlanningPackageEntryDto> Entries { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }
    public Dictionary<string, decimal> MonthlyBudget { get; set; } = new();
    public Dictionary<string, decimal> QuarterlyBudget { get; set; } = new();
    public decimal AnnualBudget { get; set; }
}