namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for package with disciplines
/// </summary>
public class PackageWithDisciplinesDto : PackageDto
{
    public List<PackageDisciplineDto> Disciplines { get; set; } = new List<PackageDisciplineDto>();
    public int TotalDisciplines { get; set; }
    public decimal TotalEstimatedHours { get; set; }
    public decimal TotalActualHours { get; set; }
    public decimal TotalEstimatedCost { get; set; }
    public decimal TotalActualCost { get; set; }
}