using System.Collections.Generic;

namespace Core.DTOs.Projects.WBSElement;

/// <summary>
/// DTO for bulk creation of WBS elements
/// </summary>
public class BulkCreateWBSElementsDto
{
    public List<CreateWBSElementDto> Elements { get; set; } = new();
}