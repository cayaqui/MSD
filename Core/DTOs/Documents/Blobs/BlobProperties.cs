using Core.Enums.Documents;

namespace Core.DTOs.Documents.Blobs
{
    public class BlobProperties
    {
        public long ContentLength { get; set; }
        public string? ContentType { get; set; }
        public string? ETag { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? CreatedOn { get; set; }
        public BlobAccessTier? AccessTier { get; set; }
        public bool IsServerEncrypted { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }

}
