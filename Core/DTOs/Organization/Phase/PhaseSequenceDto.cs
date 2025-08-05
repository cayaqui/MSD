namespace Core.DTOs.Organization.Phase;

/// <summary>
/// DTO for updating phase sequence
/// </summary>
public class PhaseSequenceDto
{
    public Guid Id { get; set; }
    public int SequenceNumber { get; set; }
}