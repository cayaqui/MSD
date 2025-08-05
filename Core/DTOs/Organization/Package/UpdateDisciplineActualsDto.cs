namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for updating discipline actuals
/// </summary>
public class UpdateDisciplineActualsDto
{
    public decimal ActualHours { get; set; }
    public decimal ActualCost { get; set; }
    public decimal ProgressPercentage { get; set; }
}