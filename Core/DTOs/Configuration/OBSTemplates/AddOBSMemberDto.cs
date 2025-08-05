namespace Core.DTOs.Configuration.OBSTemplates;

public class AddOBSMemberDto
{
    public Guid UserId { get; set; }
    public string? Role { get; set; }
    public decimal? FTEAllocation { get; set; }
}
