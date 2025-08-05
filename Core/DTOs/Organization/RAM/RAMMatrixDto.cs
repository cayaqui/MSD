namespace Core.DTOs.Organization.RAM;

/// <summary>
/// DTO for RAM matrix representation
/// </summary>
public class RAMMatrixDto
{
    public Guid ProjectId { get; set; }
    public List<RAMMatrixWBSDto> WBSElements { get; set; } = new();
    public List<RAMMatrixOBSDto> OBSNodes { get; set; } = new();
    public List<RAMMatrixAssignmentDto> Assignments { get; set; } = new();
}

/// <summary>
/// DTO for WBS element in RAM matrix
/// </summary>
public class RAMMatrixWBSDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO for OBS node in RAM matrix
/// </summary>
public class RAMMatrixOBSDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO for assignment in RAM matrix
/// </summary>
public class RAMMatrixAssignmentDto
{
    public Guid WBSElementId { get; set; }
    public Guid OBSNodeId { get; set; }
    public string ResponsibilityType { get; set; } = string.Empty;
    public decimal AllocationPercentage { get; set; }
}