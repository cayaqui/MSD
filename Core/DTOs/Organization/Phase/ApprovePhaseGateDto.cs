namespace Core.DTOs.Organization.Phase;

/// <summary>
/// Approve phase gate data transfer object
/// </summary>
public class ApprovePhaseGateDto
{
    public bool Approved { get; set; }
    public string? Notes { get; set; }
    public DateTime ReviewDate { get; set; }
}