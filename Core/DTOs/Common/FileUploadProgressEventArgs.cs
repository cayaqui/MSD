namespace Core.DTOs.Common;

public class FileUploadProgressEventArgs : EventArgs
{
    public FileUploadModel File { get; set; } = new();
    public int Progress { get; set; }
}