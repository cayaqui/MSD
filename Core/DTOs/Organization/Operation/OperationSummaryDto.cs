namespace Core.DTOs.Organization.Operation;

/// <summary>
/// Simple operation summary for company details
/// </summary>
public class OperationSummaryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int ProjectCount { get; set; }
    public int ActiveProjectCount { get; set; }
}