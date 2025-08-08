using Core.DTOs.Common;

namespace Core.DTOs.Cost.WorkPackages;

public class WorkPackageFilterDto : BaseFilterDto
{
    public Guid? ControlAccountId { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public string? Status { get; set; }
    public bool? OnlyActive { get; set; }
}