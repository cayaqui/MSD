namespace Core.DTOs.Documents.Blobs
{
    public class BlobListResult
    {
        public List<BlobItem> Items { get; set; } = new();
        public string? ContinuationToken { get; set; }
        public bool HasMore { get; set; }
    }

}
