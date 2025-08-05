using Application.Interfaces.Common;
using Core.DTOs.Contracts.ValuationItems;
using Core.DTOs.Contracts.Valuations;
using Core.Enums.Contracts;
using Domain.Entities.Contracts.Core;

namespace Application.Interfaces.Contracts;

public interface IValuationService : IBaseService<ValuationDto, CreateValuationDto, UpdateValuationDto>
{
    // Valuation queries
    Task<IEnumerable<ValuationDto>> GetByContractAsync(Guid contractId);
    Task<ValuationDto?> GetByValuationNumberAsync(string valuationNumber);
    Task<ValuationDto?> GetByPeriodAsync(Guid contractId, int period);
    Task<IEnumerable<ValuationDto>> SearchAsync(ValuationFilterDto filter);
    Task<ValuationDto?> GetLatestAsync(Guid contractId);
    Task<ValuationDto?> GetPreviousAsync(Guid valuationId);
    
    // Valuation workflow
    Task<ValuationDto?> SubmitAsync(Guid valuationId, SubmitValuationDto dto);
    Task<ValuationDto?> ApproveAsync(Guid valuationId, ApproveValuationDto dto, string approvedBy);
    Task<ValuationDto?> RejectAsync(Guid valuationId, RejectValuationDto dto, string rejectedBy);
    Task<ValuationDto?> CertifyAsync(Guid valuationId);
    
    // Valuation calculations
    Task<ValuationDto?> CalculateAmountsAsync(Guid valuationId);
    Task<ValuationDto?> RecalculateAsync(Guid valuationId);
    Task<decimal> GetCumulativeValueAsync(Guid contractId);
    
    // Valuation items
    Task<ValuationDto?> AddItemAsync(Guid valuationId, CreateValuationItemDto item);
    Task<ValuationDto?> UpdateItemAsync(Guid itemId, UpdateValuationItemDto item);
    Task<bool> RemoveItemAsync(Guid valuationId, Guid itemId);
    Task<IEnumerable<ValuationItemDto>> GetItemsAsync(Guid valuationId);
    Task<bool> ImportItemsFromPreviousAsync(Guid valuationId);
    
    // Payment processing
    Task<ValuationDto?> RecordInvoiceAsync(Guid valuationId, string invoiceNumber);
    Task<ValuationDto?> RecordPaymentAsync(Guid valuationId, decimal amount);
    Task<IEnumerable<ValuationDto>> GetUnpaidAsync(Guid? contractId = null);
    Task<IEnumerable<ValuationDto>> GetOverduePaymentsAsync(int daysOverdue = 30);
    
    // Materials tracking
    Task<ValuationDto?> UpdateMaterialsAsync(Guid valuationId, decimal onSite, decimal offSite);
    Task<decimal> GetTotalMaterialsValueAsync(Guid contractId);
    
    // Reporting
    Task<ValuationSummaryDto> GetSummaryAsync(Guid contractId);
    Task<IEnumerable<ValuationDto>> GetPendingApprovalAsync();
    Task<Dictionary<ValuationStatus, int>> GetStatusSummaryAsync(Guid? contractId = null);
    Task<decimal> GetRetentionHeldAsync(Guid contractId);
    
    // Documents
    Task<bool> AttachDocumentAsync(Guid valuationId, Guid documentId, string documentType);
    Task<bool> RemoveDocumentAsync(Guid valuationId, Guid documentId);
    Task<IEnumerable<ValuationDocument>> GetDocumentsAsync(Guid valuationId);
    
    // Export/Import
    Task<byte[]> ExportToExcelAsync(Guid valuationId);
    Task<byte[]> GenerateCertificateAsync(Guid valuationId);
    Task<ValuationDto?> CreateFromTemplateAsync(Guid contractId, Guid templateId);
}