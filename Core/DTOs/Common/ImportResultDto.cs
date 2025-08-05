namespace Core.DTOs.Common;

public class ImportResultDto
{
    public ImportResultDto()
    {
        
    }
    public ImportResultDto(int count, string message)
    {
        TotalRecords = count;
        Errors = new string[] { message };
    }
    public bool Success { get; set; }
    public int TotalRecords { get; set; }
    public int SuccessfulRecords { get; set; }
    public int FailedRecords { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
    public string[] Warnings { get; set; } = Array.Empty<string>();
    public Dictionary<string, object>? Data { get; set; }
}