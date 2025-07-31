namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for creating an Activity
/// </summary>
public class CreateActivityDto
{
    public string ActivityCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal PlannedHours { get; set; }
    public List<string>? PredecessorActivities { get; set; }
}
