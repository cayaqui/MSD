namespace Core.DTOs.Organization.Phase;

/// <summary>
/// Update phase schedule data transfer object
/// </summary>
public class UpdatePhaseScheduleDto
{
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public string? Reason { get; set; }
}