namespace Core.DTOs.Configuration.OBSTemplates;

public class OBSNodeFilterDto
{
    public Guid? ProjectId { get; set; }
    public string? NodeType { get; set; }
    public Guid? ManagerId { get; set; }
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool IncludeChildren { get; set; } = false;
    public int? MaxLevel { get; set; }
}
