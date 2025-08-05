using Core.DTOs.Cost.CommitmentWorkPackage;

namespace Core.DTOs.Cost.Commitments;

/// <summary>
/// Detailed DTO for Commitment including all related information
/// </summary>
public class CommitmentDetailDto : CommitmentDto
{
    // Related Collections
    public List<CommitmentWorkPackageDto> WorkPackageAllocations { get; set; } = new();
    public List<CommitmentRevisionDto> Revisions { get; set; } = new();
    public List<CommitmentInvoiceDto> Invoices { get; set; } = new();

    // Summary Information
    public CommitmentFinancialSummary FinancialSummary { get; set; } = new();
    public CommitmentPerformanceMetrics PerformanceMetrics { get; set; } = new();

    // Audit Trail
    public List<CommitmentAuditDto> AuditTrail { get; set; } = new();
}
