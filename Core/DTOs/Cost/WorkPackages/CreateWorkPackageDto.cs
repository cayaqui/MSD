using Core.Enums.Progress;

namespace Core.DTOs.Cost.WorkPackages;

public class CreateWorkPackageDto
{
    public Guid ProjectId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ControlAccountId { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal Budget { get; set; }
    public ProgressMethod ProgressMethod { get; set; } = ProgressMethod.Manual;
}