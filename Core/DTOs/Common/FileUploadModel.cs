namespace Core.DTOs.Common;

public class FileUploadModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public long Size { get; set; }
    public string Extension { get; set; } = "";
    public bool IsValid { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public bool IsImage { get; set; }
    public string? PreviewUrl { get; set; }
    public int UploadProgress { get; set; }
    public object? Data { get; set; }
}
