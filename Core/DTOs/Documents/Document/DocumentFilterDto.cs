using Core.Enums.Documents;

namespace Core.DTOs.Documents.Document;

public class DocumentFilterDto
{
    public Guid? ProjectId { get; set; }
    public Guid? DisciplineId { get; set; }
    public Guid? PhaseId { get; set; }
    public Guid? PackageId { get; set; }
    
    public DocumentType? Type { get; set; }
    public DocumentCategory? Category { get; set; }
    public DocumentStatus? Status { get; set; }
    public ReviewStatus? ReviewStatus { get; set; }
    public DocumentConfidentiality? Confidentiality { get; set; }
    
    public string? SearchTerm { get; set; }
    public string? DocumentNumber { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Originator { get; set; }
    
    public DateTime? IssueDateFrom { get; set; }
    public DateTime? IssueDateTo { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
    
    public List<string>? Tags { get; set; }
    public bool? IsLocked { get; set; }
    public bool? HasComments { get; set; }
    public bool? HasDistributions { get; set; }
    
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
