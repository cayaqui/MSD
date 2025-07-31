namespace Core.Dtos.Files
{

    // DTOs relacionados con archivos
    public class FileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public Guid UploadedBy { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? Tags { get; set; }
        public bool IsArchived { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
    }

    public class FileUploadDto
    {
        public Stream FileStream { get; set; } = Stream.Null;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public Guid? ProjectId { get; set; }
    }

    public class FileUploadResultDto
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public long FileSize { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class FileSearchDto
    {
        public string? SearchTerm { get; set; }
        public Guid? ProjectId { get; set; }
        public string? Category { get; set; }
        public Guid? UploadedBy { get; set; }
        public DateTime? UploadedFrom { get; set; }
        public DateTime? UploadedTo { get; set; }
        public long? MinSize { get; set; }
        public long? MaxSize { get; set; }
        public List<string>? ContentTypes { get; set; }
        public bool? IncludeArchived { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }

    public class FileCategoryDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> AllowedExtensions { get; set; } = new();
        public long? MaxFileSize { get; set; }
        public int FileCount { get; set; }
    }

    public class UpdateFileMetadataDto
    {
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
    }

    public class ImageProcessingOptions
    {
        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
        public bool MaintainAspectRatio { get; set; } = true;
        public int Quality { get; set; } = 85;
        public bool GenerateThumbnail { get; set; } = true;
        public int ThumbnailWidth { get; set; } = 150;
        public int ThumbnailHeight { get; set; } = 150;
    }
}