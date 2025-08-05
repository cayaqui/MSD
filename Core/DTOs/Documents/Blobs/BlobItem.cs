namespace Core.DTOs.Documents.Blobs
{
    public class BlobItem
    {
        public string Name { get; set; } = string.Empty;
        public string? ContainerName { get; set; }
        public long? Size { get; set; }
        public string? ContentType { get; set; }
        public DateTime? LastModified { get; set; }
        public string? ETag { get; set; }
        public bool IsDeleted { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }

}
