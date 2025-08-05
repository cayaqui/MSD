namespace Core.DTOs.Configuration.SystemParameters;

public class ParameterHistoryDto
{
    public Guid Id { get; set; }
    public Guid ParameterId { get; set; }
    public string ParameterKey { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string? ChangeReason { get; set; }
    public string? IpAddress { get; set; }
}