using Core.DTOs.Documents.DocumentDistribution;
using Core.Enums.Documents;

namespace Application.Interfaces.Documents;

public interface IDocumentDistributionService
{
    // Distribution management
    Task<DocumentDistributionDto?> CreateDistributionAsync(CreateDocumentDistributionDto dto);
    Task<IEnumerable<DocumentDistributionDto>> GetDocumentDistributionsAsync(Guid documentId);
    Task<IEnumerable<DocumentDistributionDto>> GetUserDistributionsAsync(Guid userId);
    Task<IEnumerable<DocumentDistributionDto>> GetCompanyDistributionsAsync(Guid companyId);
    
    // Acknowledgment
    Task<bool> AcknowledgeDistributionAsync(Guid distributionId, string acknowledgedBy, string? comments = null);
    Task<IEnumerable<DocumentDistributionDto>> GetPendingAcknowledgmentsAsync(Guid userId);
    
    // Download tracking
    Task RecordDownloadAsync(Guid distributionId);
    Task<Stream?> DownloadDistributedDocumentAsync(Guid distributionId, string? accessToken = null);
    Task<string?> GetDistributionDownloadUrlAsync(Guid distributionId, int expiryMinutes = 60);
    
    // Bulk distribution
    Task<IEnumerable<DocumentDistributionDto>> DistributeMultipleAsync(List<CreateDocumentDistributionDto> distributions);
    Task<bool> ResendDistributionAsync(Guid distributionId);
    
    // Reporting
    Task<DistributionReportDto> GetDistributionReportAsync(Guid documentId);
    Task<IEnumerable<DistributionStatisticsDto>> GetDistributionStatisticsAsync(Guid projectId, DateTime? fromDate = null, DateTime? toDate = null);
}
