namespace Core.DTOs.Organization.Operation;

/// <summary>
/// DTO for updating operation address
/// </summary>
public class UpdateOperationAddressDto
{
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}