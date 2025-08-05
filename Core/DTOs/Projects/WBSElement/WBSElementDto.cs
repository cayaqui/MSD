using Core.Enums.Projects;
using Core.Enums.Progress;

namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for WBS Element list view
/// </summary>
public class WBSElementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public int SequenceNumber { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public WBSElementType ElementType { get; set; }
    public Guid? ControlAccountId { get; set; }
    public string? ControlAccountCode { get; set; }
    public bool IsActive { get; set; }
    public int ChildrenCount { get; set; }
    public bool CanHaveChildren { get; set; }
    public decimal? ProgressPercentage { get; set; }
    public WorkPackageStatus? Status { get; set; }
}
