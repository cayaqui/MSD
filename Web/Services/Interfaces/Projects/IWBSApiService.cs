using Core.DTOs.Common;
using Core.DTOs.Projects.WBSElement;
using Core.DTOs.Projects.WorkPackageDetails;
using Core.Enums.Projects;
using Core.Enums.Documents;

namespace Web.Services.Interfaces.Projects;

public interface IWBSApiService
{
    Task<List<WBSElementDto>> GetWBSHierarchyAsync(Guid projectId);
    Task<WBSElementDetailDto> GetWBSElementByIdAsync(Guid id);
    Task<Guid> CreateWBSElementAsync(CreateWBSElementDto dto);
    Task UpdateWBSElementAsync(Guid id, UpdateWBSElementDto dto);
    Task DeleteWBSElementAsync(Guid id);
    Task<WBSDictionaryDto> GetWBSDictionaryAsync(Guid elementId);
    Task UpdateWBSDictionaryAsync(Guid elementId, UpdateWBSDictionaryDto dto);
    Task ConvertToWorkPackageAsync(Guid elementId, ConvertToWorkPackageDto dto);
    Task ConvertToPlanningPackageAsync(Guid elementId, Guid controlAccountId);
    Task<ImportResultDto> ImportWBSAsync(Guid projectId, Stream fileStream, string fileName);
    Task<byte[]> ExportWBSAsync(Guid projectId, ExportFormat format = ExportFormat.Excel);
    Task<byte[]> DownloadWBSTemplateAsync();
    Task ReorderWBSElementsAsync(ReorderWBSElementsDto dto);
}