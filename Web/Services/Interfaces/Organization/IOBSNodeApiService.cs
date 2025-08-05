using Core.DTOs.Common;
using Core.DTOs.Organization.OBSNode;
using Core.DTOs.Auth.ProjectTeamMembers;

namespace Web.Services.Interfaces.Organization;

/// <summary>
/// Service interface for OBS (Organizational Breakdown Structure) node API operations
/// </summary>
public interface IOBSNodeApiService
{
    // Query Operations
    
    /// <summary>
    /// Get all OBS nodes with pagination
    /// </summary>
    Task<PagedResult<OBSNodeDto>?> GetOBSNodesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get OBS node by ID
    /// </summary>
    Task<OBSNodeDto?> GetOBSNodeByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get OBS nodes by project
    /// </summary>
    Task<List<OBSNodeDto>?> GetOBSNodesByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get OBS hierarchy for a project
    /// </summary>
    Task<OBSNodeHierarchyDto?> GetOBSHierarchyAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get OBS node children
    /// </summary>
    Task<List<OBSNodeDto>?> GetOBSNodeChildrenAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get OBS node team members
    /// </summary>
    Task<OBSNodeTeamDto?> GetOBSNodeTeamAsync(Guid id, CancellationToken cancellationToken = default);

    // Command Operations
    
    /// <summary>
    /// Create a new OBS node
    /// </summary>
    Task<OBSNodeDto?> CreateOBSNodeAsync(CreateOBSNodeDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing OBS node
    /// </summary>
    Task<OBSNodeDto?> UpdateOBSNodeAsync(Guid id, UpdateOBSNodeDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update OBS node manager
    /// </summary>
    Task<OBSNodeDto?> UpdateOBSNodeManagerAsync(Guid id, UpdateOBSNodeManagerDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update OBS node cost center
    /// </summary>
    Task<OBSNodeDto?> UpdateOBSNodeCostCenterAsync(Guid id, UpdateOBSNodeCostCenterDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Move OBS node to a new parent
    /// </summary>
    Task<OBSNodeDto?> MoveOBSNodeAsync(Guid id, MoveOBSNodeDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Bulk update OBS nodes
    /// </summary>
    Task<List<OBSNodeDto>?> BulkUpdateOBSNodesAsync(List<UpdateOBSNodeDto> dtos, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete OBS node (soft delete)
    /// </summary>
    Task<bool> DeleteOBSNodeAsync(Guid id, CancellationToken cancellationToken = default);
}