

using Core.DTOs.Projects.WBSElement;
using Core.DTOs.Projects.WorkPackageDetails;

namespace Application.Interfaces.Projects;

/// <summary>
/// Service interface for Work Breakdown Structure operations
/// </summary>
public interface IWBSService
{
    // Query operations
    Task<PagedResult<WBSElementDto>> GetWBSElementsAsync(
        Guid projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<WBSElementDetailDto?> GetWBSElementByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<WBSElementDto>> GetWBSHierarchyAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<List<WBSElementDto>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default);

    Task<List<WBSElementDto>> GetWorkPackagesByControlAccountAsync(
        Guid controlAccountId,
        CancellationToken cancellationToken = default);

    Task<WBSDictionaryDto?> GetWBSDictionaryAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default);

    // Command operations
    Task<Result<Guid>> CreateWBSElementAsync(
        CreateWBSElementDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateWBSElementAsync(
        Guid id,
        UpdateWBSElementDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateWBSDictionaryAsync(
        Guid id,
        UpdateWBSDictionaryDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> ConvertToWorkPackageAsync(
        Guid id,
        ConvertToWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> ConvertToPlanningPackageAsync(
        Guid id,
        Guid controlAccountId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> ConvertPlanningPackageToWorkPackageAsync(
        Guid id,
        ConvertPlanningToWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> ReorderWBSElementsAsync(
        ReorderWBSElementsDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteWBSElementAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    // Validation operations
    Task<bool> CanConvertToWorkPackageAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default);

    Task<bool> CanDeleteWBSElementAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default);

    Task<Application.Interfaces.Common.ValidationResult> ValidateWBSCodeAsync(
        string code,
        Guid projectId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    // Bulk operations
    Task<BulkOperationResult> BulkCreateWBSElementsAsync(
        BulkCreateWBSElementsDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    // Import/Export operations
    Task<ImportResult> ImportWBSAsync(
        Stream fileStream,
        string fileName,
        Guid projectId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportWBSAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
}