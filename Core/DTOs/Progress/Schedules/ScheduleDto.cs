using Core.DTOs.Common;
using Core.Enums.Progress;

namespace Core.DTOs.Progress.Schedules;

public class ScheduleDto : BaseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ScheduleType Type { get; set; }
    public ScheduleStatus Status { get; set; }
    public int Version { get; set; }
    public bool IsBaseline { get; set; }
    public DateTime? BaselineDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalActivities { get; set; }
    public int CompletedActivities { get; set; }
    public decimal OverallProgress { get; set; }
    public int CriticalActivities { get; set; }
    public decimal FloatThreshold { get; set; }
    public string? CalendarId { get; set; }
    public bool AutoCalculate { get; set; }
    public string? Notes { get; set; }
    
    // Calculated properties
    public int Duration => (EndDate - StartDate).Days;
    public decimal CompletionPercentage => TotalActivities > 0 ? (decimal)CompletedActivities / TotalActivities * 100 : 0;
}