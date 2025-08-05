using Core.DTOs.Documents.Document;
using Core.DTOs.Documents.DocumentComment;
using Core.DTOs.Documents.DocumentVersion;
using Core.Enums.Documents;


namespace Application.Interfaces.Documents;

public interface IDocumentService : IBaseService<DocumentDto, CreateDocumentDto, UpdateDocumentDto>
{
    // Document retrieval
    Task<DocumentDto?> GetByDocumentNumberAsync(string documentNumber);
    Task<IEnumerable<DocumentDto>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<DocumentDto>> GetByDisciplineAsync(Guid disciplineId);
    Task<IEnumerable<DocumentDto>> GetByPhaseAsync(Guid phaseId);
    Task<IEnumerable<DocumentDto>> GetByPackageAsync(Guid packageId);
    
    // Search and filtering
    Task<DocumentSearchResultDto> SearchDocumentsAsync(DocumentFilterDto filter);
    Task<IEnumerable<DocumentDto>> GetRecentDocumentsAsync(Guid projectId, int count = 10);
    Task<IEnumerable<DocumentDto>> GetMyDocumentsAsync(Guid userId);
    Task<IEnumerable<DocumentDto>> GetSharedWithMeAsync(Guid userId);
    
    // Version control
    Task<DocumentVersionDto?> CreateVersionAsync(CreateDocumentVersionDto dto);
    Task<IEnumerable<DocumentVersionDto>> GetVersionsAsync(Guid documentId);
    Task<DocumentVersionDto?> GetVersionAsync(Guid documentId, string version);
    Task<bool> SetCurrentVersionAsync(Guid documentId, Guid versionId);
    Task<bool> SupersedeDocumentAsync(Guid documentId, Guid newDocumentId);
    
    // File operations
    Task<DocumentDto?> UploadDocumentAsync(CreateDocumentDto dto, Stream fileStream, string fileName, string contentType);
    Task<Stream?> DownloadDocumentAsync(Guid documentId);
    Task<Stream?> DownloadVersionAsync(Guid versionId);
    Task<string?> GetDownloadUrlAsync(Guid documentId, int expiryMinutes = 60);
    Task<string?> GetVersionDownloadUrlAsync(Guid versionId, int expiryMinutes = 60);
    
    // Document management
    Task<bool> LockDocumentAsync(Guid documentId, Guid userId);
    Task<bool> UnlockDocumentAsync(Guid documentId, Guid userId);
    Task<bool> CheckOutAsync(Guid documentId, Guid userId);
    Task<bool> CheckInAsync(Guid documentId, Guid userId, string? comments = null);
    Task<bool> UpdateMetadataAsync(Guid documentId, Dictionary<string, string> metadata);
    
    // Access control
    Task<bool> GrantAccessAsync(Guid documentId, Guid userId, DocumentPermissionDto permissions);
    Task<bool> RevokeAccessAsync(Guid documentId, Guid userId);
    Task<IEnumerable<DocumentPermissionDto>> GetPermissionsAsync(Guid documentId);
    Task<bool> HasPermissionAsync(Guid documentId, Guid userId, string permission);
    
    // Review and approval
    Task<bool> SubmitForReviewAsync(Guid documentId, List<Guid> reviewerIds);
    Task<bool> ApproveDocumentAsync(Guid documentId, Guid reviewerId, string? comments = null);
    Task<bool> RejectDocumentAsync(Guid documentId, Guid reviewerId, string comments);
    Task<IEnumerable<DocumentDto>> GetPendingReviewsAsync(Guid reviewerId);
    
    // Comments
    Task<DocumentCommentDto?> AddCommentAsync(CreateDocumentCommentDto dto);
    Task<IEnumerable<DocumentCommentDto>> GetCommentsAsync(Guid documentId);
    Task<bool> ResolveCommentAsync(Guid commentId, Guid userId, string? resolution = null);
    
    // Relationships
    Task<bool> LinkDocumentsAsync(Guid documentId, Guid relatedDocumentId, DocumentRelationshipType type);
    Task<bool> UnlinkDocumentsAsync(Guid documentId, Guid relatedDocumentId);
    Task<IEnumerable<DocumentDto>> GetRelatedDocumentsAsync(Guid documentId);
    
    // Statistics and analytics
    Task RecordViewAsync(Guid documentId, Guid userId);
    Task RecordDownloadAsync(Guid documentId, Guid userId);
    Task<DocumentStatisticsDto> GetStatisticsAsync(Guid documentId);
    
    // Bulk operations
    Task<IEnumerable<DocumentDto>> CreateBulkAsync(List<CreateDocumentDto> documents);
    Task<bool> UpdateBulkStatusAsync(List<Guid> documentIds, DocumentStatus status);
    Task<bool> DeleteBulkAsync(List<Guid> documentIds);
    
    // Export/Import
    Task<byte[]> ExportDocumentListAsync(DocumentFilterDto filter, ExportFormat format);
    Task<IEnumerable<DocumentDto>> ImportDocumentsAsync(Stream fileStream, string contentType);
}

public class DocumentPermissionDto
{
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? RoleName { get; set; }
    public bool CanView { get; set; }
    public bool CanDownload { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanComment { get; set; }
    public bool CanDistribute { get; set; }
    public bool CanManagePermissions { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class DocumentStatisticsDto
{
    public int TotalViews { get; set; }
    public int TotalDownloads { get; set; }
    public int UniqueViewers { get; set; }
    public int UniqueDownloaders { get; set; }
    public DateTime? LastViewed { get; set; }
    public DateTime? LastDownloaded { get; set; }
    public int CommentCount { get; set; }
    public int VersionCount { get; set; }
    public int DistributionCount { get; set; }
    public Dictionary<string, int> ViewsByDate { get; set; } = new();
    public Dictionary<string, int> DownloadsByDate { get; set; } = new();
}

