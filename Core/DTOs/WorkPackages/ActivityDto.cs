using Core.Enums.Progress;

namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for Activity
/// </summary>
public class ActivityDto
{
    public Guid Id { get; set; }
    public string ActivityCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int DurationDays { get; set; }
    public decimal ProgressPercentage { get; set; }
    public ActivityStatus Status { get; set; }
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
}
