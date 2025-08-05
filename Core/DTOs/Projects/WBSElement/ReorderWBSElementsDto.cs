namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for reordering WBS Elements
/// </summary>
public class ReorderWBSElementsDto
{
    public Guid ParentId { get; set; }
    public List<WBSElementOrderDto> Elements { get; set; } = new();
}
