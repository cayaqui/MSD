namespace Core.DTOs.Documents.Blobs
{
    public class BlobDownloadResult
    {
        public Stream? Content { get; set; }
        public string? ContentType { get; set; }
        public long ContentLength { get; set; }
        public string? ETag { get; set; }
        public DateTime? LastModified { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }

}
