namespace Core.DTOs.Organization.Company;

/// <summary>
/// DTO for updating company logo
/// </summary>
public class UpdateCompanyLogoDto
{
    public byte[] Logo { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
}