namespace Core.DTOs.Common;

public class FileUploadEventArgs : EventArgs
{
    public FileUploadModel File { get; set; } = new();
}
