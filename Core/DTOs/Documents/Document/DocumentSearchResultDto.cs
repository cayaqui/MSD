using Core.Enums.Documents;

namespace Core.DTOs.Documents.Document;

public class DocumentSearchResultDto
{
    public List<DocumentDto> Documents { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    
    // Facets for filtering
    public Dictionary<DocumentType, int> TypeCounts { get; set; } = new();
    public Dictionary<DocumentCategory, int> CategoryCounts { get; set; } = new();
    public Dictionary<DocumentStatus, int> StatusCounts { get; set; } = new();
    public List<string> AvailableTags { get; set; } = new();
}