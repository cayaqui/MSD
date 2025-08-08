using Core.DTOs.Common;
using Core.DTOs.Documents.Document;
using Core.DTOs.Documents.DocumentComment;
using Core.DTOs.Documents.DocumentVersion;
using Core.Enums.Documents;
using Microsoft.AspNetCore.Components.Forms;

namespace Web.Services.Interfaces.Documents;

/// <summary>
/// Service interface for document API operations
/// </summary>
public interface IDocumentApiService
{
    // Query Operations
    
    /// <summary>
    /// Search documents with filters
    /// </summary>
    Task<DocumentSearchResultDto?> SearchDocumentsAsync(DocumentFilterDto filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get document by ID
    /// </summary>
    Task<DocumentDto?> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get document by document number
    /// </summary>
    Task<DocumentDto?> GetDocumentByNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all documents for a project
    /// </summary>
    Task<IEnumerable<DocumentDto>?> GetProjectDocumentsAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get recent documents for a project
    /// </summary>
    Task<IEnumerable<DocumentDto>?> GetRecentDocumentsAsync(Guid projectId, int count = 10, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get documents created by current user
    /// </summary>
    Task<IEnumerable<DocumentDto>?> GetMyDocumentsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get documents shared with current user
    /// </summary>
    Task<IEnumerable<DocumentDto>?> GetSharedWithMeAsync(CancellationToken cancellationToken = default);
    
    // Command Operations
    
    /// <summary>
    /// Create a new document
    /// </summary>
    Task<DocumentDto?> CreateDocumentAsync(CreateDocumentDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing document
    /// </summary>
    Task<DocumentDto?> UpdateDocumentAsync(Guid documentId, UpdateDocumentDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete a document
    /// </summary>
    Task<bool> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    // File Operations
    
    /// <summary>
    /// Upload a document file
    /// </summary>
    Task<DocumentDto?> UploadDocumentAsync(CreateDocumentDto dto, IBrowserFile file, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Download a document
    /// </summary>
    Task<byte[]?> DownloadDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get temporary download URL
    /// </summary>
    Task<string?> GetDownloadUrlAsync(Guid documentId, int expiryMinutes = 60, CancellationToken cancellationToken = default);
    
    // Version Control
    
    /// <summary>
    /// Create a new version of a document
    /// </summary>
    Task<DocumentVersionDto?> CreateVersionAsync(Guid documentId, CreateDocumentVersionDto dto, IBrowserFile file, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all versions of a document
    /// </summary>
    Task<IEnumerable<DocumentVersionDto>?> GetVersionsAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get specific version of a document
    /// </summary>
    Task<DocumentVersionDto?> GetVersionAsync(Guid documentId, string version, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Set a version as current
    /// </summary>
    Task<bool> SetCurrentVersionAsync(Guid documentId, Guid versionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Download specific version
    /// </summary>
    Task<byte[]?> DownloadVersionAsync(Guid versionId, CancellationToken cancellationToken = default);
    
    // Document Management
    
    /// <summary>
    /// Lock a document for editing
    /// </summary>
    Task<bool> LockDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unlock a document
    /// </summary>
    Task<bool> UnlockDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check out a document
    /// </summary>
    Task<bool> CheckOutDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check in a document
    /// </summary>
    Task<bool> CheckInDocumentAsync(Guid documentId, string? comments = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update document metadata
    /// </summary>
    Task<bool> UpdateMetadataAsync(Guid documentId, Dictionary<string, string> metadata, CancellationToken cancellationToken = default);
    
    // Access Control
    
    /// <summary>
    /// Grant access to a document
    /// </summary>
    Task<bool> GrantAccessAsync(Guid documentId, Guid userId, DocumentPermissionDto permissions, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Revoke access to a document
    /// </summary>
    Task<bool> RevokeAccessAsync(Guid documentId, Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get document permissions
    /// </summary>
    Task<IEnumerable<DocumentPermissionDto>?> GetPermissionsAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    // Review and Approval
    
    /// <summary>
    /// Submit document for review
    /// </summary>
    Task<bool> SubmitForReviewAsync(Guid documentId, List<Guid> reviewerIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Approve a document
    /// </summary>
    Task<bool> ApproveDocumentAsync(Guid documentId, string? comments = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reject a document
    /// </summary>
    Task<bool> RejectDocumentAsync(Guid documentId, string comments, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get documents pending review
    /// </summary>
    Task<IEnumerable<DocumentDto>?> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
    
    // Comments
    
    /// <summary>
    /// Add comment to document
    /// </summary>
    Task<DocumentCommentDto?> AddCommentAsync(Guid documentId, CreateDocumentCommentDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get document comments
    /// </summary>
    Task<IEnumerable<DocumentCommentDto>?> GetCommentsAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Resolve a comment
    /// </summary>
    Task<bool> ResolveCommentAsync(Guid commentId, string? resolution = null, CancellationToken cancellationToken = default);
    
    // Relationships
    
    /// <summary>
    /// Link documents
    /// </summary>
    Task<bool> LinkDocumentsAsync(Guid documentId, Guid relatedDocumentId, DocumentRelationshipType relationshipType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unlink documents
    /// </summary>
    Task<bool> UnlinkDocumentsAsync(Guid documentId, Guid relatedDocumentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get related documents
    /// </summary>
    Task<IEnumerable<DocumentDto>?> GetRelatedDocumentsAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    // Statistics
    
    /// <summary>
    /// Record document view
    /// </summary>
    Task RecordViewAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get document statistics
    /// </summary>
    Task<DocumentStatisticsDto?> GetStatisticsAsync(Guid documentId, CancellationToken cancellationToken = default);
    
    // Bulk Operations
    
    /// <summary>
    /// Create multiple documents
    /// </summary>
    Task<IEnumerable<DocumentDto>?> CreateBulkAsync(List<CreateDocumentDto> documents, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update status for multiple documents
    /// </summary>
    Task<bool> UpdateBulkStatusAsync(List<Guid> documentIds, DocumentStatus status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete multiple documents
    /// </summary>
    Task<bool> DeleteBulkAsync(List<Guid> documentIds, CancellationToken cancellationToken = default);
    
    // Export/Import
    
    /// <summary>
    /// Export document list
    /// </summary>
    Task<byte[]?> ExportDocumentsAsync(DocumentFilterDto filter, ExportFormat format, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Import documents from file
    /// </summary>
    Task<IEnumerable<DocumentDto>?> ImportDocumentsAsync(IBrowserFile file, CancellationToken cancellationToken = default);
}