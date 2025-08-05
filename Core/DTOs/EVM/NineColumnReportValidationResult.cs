namespace Core.DTOs.EVM;

/// <summary>
/// Result of Nine Column Report validation
/// </summary>
public class NineColumnReportValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<Guid, List<string>> ItemErrors { get; set; } = new(); // Errors by Control Account ID

    // Validation statistics
    public int TotalControlAccounts { get; set; }
    public int ControlAccountsWithErrors { get; set; }
    public int MissingEVMData { get; set; }
    public int InconsistentCalculations { get; set; }
}