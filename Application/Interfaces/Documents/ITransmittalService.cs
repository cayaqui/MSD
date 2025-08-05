using Core.DTOs.Documents.Transmittal;
using Core.DTOs.Documents.TransmittalAttachment;
using Core.DTOs.Documents.TransmittalDocument;
using Core.DTOs.Documents.TransmittalRecipient;
using Core.Enums.Documents;

namespace Application.Interfaces.Documents;

public interface ITransmittalService : IBaseService<TransmittalDto, CreateTransmittalDto, UpdateTransmittalDto>
{
    // Transmittal management
    Task<TransmittalDto?> GetByTransmittalNumberAsync(string transmittalNumber);
    Task<IEnumerable<TransmittalDto>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<TransmittalDto>> GetSentTransmittalsAsync(Guid companyId);
    Task<IEnumerable<TransmittalDto>> GetReceivedTransmittalsAsync(Guid companyId);
    Task<string> GenerateTransmittalNumberAsync(Guid projectId);
    
    // Document management
    Task<bool> AddDocumentAsync(Guid transmittalId, TransmittalDocumentItemDto document);
    Task<bool> RemoveDocumentAsync(Guid transmittalId, Guid documentId);
    Task<bool> UpdateDocumentAsync(Guid transmittalId, Guid documentId, TransmittalDocumentItemDto update);
    Task<IEnumerable<TransmittalDocumentDto>> GetDocumentsAsync(Guid transmittalId);
    
    // Recipient management
    Task<bool> AddRecipientAsync(Guid transmittalId, CreateTransmittalRecipientDto recipient);
    Task<bool> RemoveRecipientAsync(Guid transmittalId, Guid recipientId);
    Task<IEnumerable<TransmittalRecipientDto>> GetRecipientsAsync(Guid transmittalId);
    
    // Attachment management
    Task<TransmittalAttachmentDto?> AddAttachmentAsync(Guid transmittalId, Stream fileStream, string fileName, string contentType, string? description = null);
    Task<bool> RemoveAttachmentAsync(Guid transmittalId, Guid attachmentId);
    Task<Stream?> DownloadAttachmentAsync(Guid attachmentId);
    Task<IEnumerable<TransmittalAttachmentDto>> GetAttachmentsAsync(Guid transmittalId);
    
    // Workflow
    Task<TransmittalDto?> SubmitForApprovalAsync(Guid transmittalId);
    Task<TransmittalDto?> ApproveAsync(Guid transmittalId, Guid approverId, string? comments = null);
    Task<TransmittalDto?> RejectAsync(Guid transmittalId, Guid approverId, string comments);
    Task<TransmittalDto?> SendAsync(Guid transmittalId, Guid sentById);
    Task<TransmittalDto?> DeliverAsync(Guid transmittalId, string confirmedBy);
    Task<TransmittalDto?> AcknowledgeAsync(Guid transmittalId, Guid recipientId, string? comments = null);
    
    // Status tracking
    Task<bool> UpdateDeliveryStatusAsync(Guid transmittalId, bool isDelivered, string? trackingInfo = null);
    Task<IEnumerable<TransmittalDto>> GetPendingDeliveryAsync();
    Task<IEnumerable<TransmittalDto>> GetPendingAcknowledgmentAsync();
    
    // Reporting
    Task<TransmittalReportDto> GenerateTransmittalReportAsync(Guid transmittalId);
    Task<byte[]> ExportTransmittalAsync(Guid transmittalId, ExportFormat format);
    Task<byte[]> ExportTransmittalListAsync(TransmittalFilterDto filter, ExportFormat format);
    Task<TransmittalStatisticsDto> GetStatisticsAsync(Guid projectId, DateTime? fromDate = null, DateTime? toDate = null);
    
    // Bulk operations
    Task<IEnumerable<TransmittalDto>> CreateBulkTransmittalsAsync(List<CreateTransmittalDto> transmittals);
    Task<bool> SendBulkAsync(List<Guid> transmittalIds, Guid sentById);
    
    // Templates
    Task<TransmittalDto?> CreateFromTemplateAsync(Guid templateId, CreateTransmittalFromTemplateDto dto);
    Task<bool> SaveAsTemplateAsync(Guid transmittalId, string templateName, string? description = null);
}

public class TransmittalReportDto
{
    public TransmittalDto Transmittal { get; set; } = null!;
    public List<TransmittalDocumentDto> Documents { get; set; } = new();
    public List<TransmittalRecipientDto> Recipients { get; set; } = new();
    public List<TransmittalAttachmentDto> Attachments { get; set; } = new();
    public string? CoverLetter { get; set; }
    public DateTime GeneratedDate { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
}

public class TransmittalFilterDto
{
    public Guid? ProjectId { get; set; }
    public Guid? FromCompanyId { get; set; }
    public Guid? ToCompanyId { get; set; }
    public TransmittalStatus? Status { get; set; }
    public TransmittalPriority? Priority { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? SearchTerm { get; set; }
    public bool IncludeDeleted { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class TransmittalStatisticsDto
{
    public int TotalTransmittals { get; set; }
    public int SentTransmittals { get; set; }
    public int ReceivedTransmittals { get; set; }
    public int PendingApproval { get; set; }
    public int PendingDelivery { get; set; }
    public int PendingAcknowledgment { get; set; }
    public int TotalDocuments { get; set; }
    public Dictionary<TransmittalStatus, int> StatusCounts { get; set; } = new();
    public Dictionary<DeliveryMethod, int> DeliveryMethodCounts { get; set; } = new();
    public Dictionary<string, int> TransmittalsByMonth { get; set; } = new();
    public double AverageResponseTime { get; set; }
    public double AcknowledgmentRate { get; set; }
}

public class CreateTransmittalFromTemplateDto
{
    public Guid ProjectId { get; set; }
    public Guid ToCompanyId { get; set; }
    public string? ToContact { get; set; }
    public string? ToEmail { get; set; }
    public List<Guid> DocumentIds { get; set; } = new();
    public string? Subject { get; set; }
    public string? Description { get; set; }
}