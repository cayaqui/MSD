namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for updating WBS Element
/// </summary>
public class UpdateWBSElementDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
