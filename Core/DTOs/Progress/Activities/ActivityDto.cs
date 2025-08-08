using Core.DTOs.Common;
using Core.Enums.Progress;

namespace Core.DTOs.Progress.Activities;

public class ActivityDto : BaseDto
{
    public Guid WBSElementId { get; set; }
    public string ActivityCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Schedule
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int DurationDays { get; set; }
    
    // Progress
    public decimal ProgressPercentage { get; set; }
    public ActivityStatus Status { get; set; }
    public string StatusDisplay => Status switch
    {
        ActivityStatus.NotStarted => "No Iniciada",
        ActivityStatus.InProgress => "En Progreso",
        ActivityStatus.Completed => "Completada",
        ActivityStatus.Suspended => "Suspendida",
        ActivityStatus.Cancelled => "Cancelada",
        _ => Status.ToString()
    };
    
    // Resources
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal? ResourceRate { get; set; }
    
    // Dependencies
    public string[]? PredecessorActivities { get; set; }
    public string[]? SuccessorActivities { get; set; }
    
    // Related Info
    public string WBSCode { get; set; } = string.Empty;
    public string WBSName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    
    // Metrics
    public bool IsOnSchedule { get; set; }
    public int DelayDays { get; set; }
    public decimal ProductivityRate { get; set; }
}

public class CreateActivityDto
{
    public Guid WBSElementId { get; set; }
    public string ActivityCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal PlannedHours { get; set; }
    public decimal? ResourceRate { get; set; }
    public string[]? Predecessors { get; set; }
    public string[]? Successors { get; set; }
}

public class UpdateActivityDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public decimal? PlannedHours { get; set; }
    public decimal? ResourceRate { get; set; }
    public string[]? PredecessorActivities { get; set; }
    public string[]? SuccessorActivities { get; set; }
}

public class UpdateActivityProgressDto
{
    public decimal ProgressPercentage { get; set; }
    public decimal ActualHours { get; set; }
    public string? Notes { get; set; }
}

public class ActivityFilterDto : BaseFilterDto
{
    public Guid? ProjectId { get; set; }
    public Guid? WBSElementId { get; set; }
    public ActivityStatus? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public decimal? MinProgress { get; set; }
    public decimal? MaxProgress { get; set; }
    public bool? IsCritical { get; set; }
}

public class BulkUpdateActivitiesDto
{
    public List<Guid> ActivityIds { get; set; } = new();
    public List<ActivityProgressUpdate> Updates { get; set; } = new();
}

public class ActivityProgressUpdate
{
    public Guid ActivityId { get; set; }
    public decimal ProgressPercentage { get; set; }
    public decimal ActualHours { get; set; }
}