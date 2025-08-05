using Core.DTOs.Common;
using Core.DTOs.Reports;
using Core.Enums.Documents;
using Core.Enums.Reports;

namespace Application.Interfaces.Reports;

public interface IReportTemplateService : IBaseService<ReportTemplateDto, CreateReportTemplateDto, UpdateReportTemplateDto>
{
    Task<IEnumerable<ReportTemplateDto>> GetActiveTemplatesAsync();
    Task<IEnumerable<ReportTemplateDto>> GetTemplatesByTypeAsync(ReportType type);
    Task<IEnumerable<ReportTemplateDto>> GetTemplatesByCategoryAsync(string category);
    Task<ReportTemplateDto?> GetDefaultTemplateAsync(ReportType type);
    Task<ReportTemplateDto?> SetAsDefaultAsync(Guid templateId);
    Task<ReportTemplateDto?> ActivateTemplateAsync(Guid templateId);
    Task<ReportTemplateDto?> DeactivateTemplateAsync(Guid templateId);
    Task<byte[]?> GetTemplateFileAsync(Guid templateId);
    Task<ReportTemplateDto?> UploadTemplateFileAsync(Guid templateId, byte[] fileContent, string fileName);
    Task<bool> ValidateTemplateAsync(Guid templateId);
    Task<ReportTemplateDto?> CloneTemplateAsync(Guid templateId, string newName, string newCode);
}
