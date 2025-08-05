using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Documents.Blobs
{

    public class BlobUploadResult
    {
        public bool Success { get; set; }
        public string? BlobUrl { get; set; }
        public string? BlobName { get; set; }
        public string? ContainerName { get; set; }
        public string? ETag { get; set; }
        public DateTime? LastModified { get; set; }
        public long? ContentLength { get; set; }
        public string? ContentType { get; set; }
        public string? ErrorMessage { get; set; }
    }

}
