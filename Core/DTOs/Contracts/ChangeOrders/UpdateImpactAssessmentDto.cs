using Core.Enums.Change;

namespace Core.DTOs.Contracts.ChangeOrders;

public class UpdateImpactAssessmentDto
{
    public string ImpactAssessment { get; set; } = string.Empty;
    public ImpactCategory ImpactCategory { get; set; }
    public ImpactSeverity ImpactSeverity { get; set; }
    public int ScheduleImpactDays { get; set; }
}