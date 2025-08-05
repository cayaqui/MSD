using Core.Enums.Projects;

namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for creating WBS Element
/// </summary>
public class CreateWBSElementDto
{
    public Guid ProjectId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public WBSElementType ElementType { get; set; }
    public int? SequenceNumber { get; set; }
}
