using Core.DTOs.Configuration.OBSTemplates;
using Core.DTOs.Organization.OBSNode;

namespace Application.Interfaces.Configuration;

/// <summary>
/// Service for managing Organization Breakdown Structure
/// </summary>
public interface IOBSService : IBaseService<OBSNodeDto, CreateOBSNodeDto, UpdateOBSNodeDto>
{
    
    /// <summary>
    /// Get OBS nodes by project
    /// </summary>
    Task<IEnumerable<OBSNodeDto>> GetByProjectAsync(Guid projectId);
    
    /// <summary>
    /// Get root nodes
    /// </summary>
    Task<IEnumerable<OBSNodeDto>> GetRootNodesAsync(Guid? projectId = null);
    
    /// <summary>
    /// Get child nodes
    /// </summary>
    Task<IEnumerable<OBSNodeDto>> GetChildrenAsync(Guid parentId);
    
    /// <summary>
    /// Get node hierarchy
    /// </summary>
    Task<OBSNodeDetailDto?> GetHierarchyAsync(Guid nodeId);
    
    /// <summary>
    /// Search OBS nodes
    /// </summary>
    Task<IEnumerable<OBSNodeDto>> SearchAsync(OBSNodeFilterDto filter);
    
    /// <summary>
    /// Create OBS node
    /// </summary>
    Task<OBSNodeDto> CreateAsync(CreateOBSNodeDto dto);
    
    /// <summary>
    /// Update OBS node
    /// </summary>
    Task<OBSNodeDto?> UpdateAsync(Guid id, UpdateOBSNodeDto dto);
    
    /// <summary>
    /// Delete OBS node
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
    
    /// <summary>
    /// Move node to new parent
    /// </summary>
    Task<bool> MoveNodeAsync(MoveOBSNodeDto dto);
    
    /// <summary>
    /// Add member to OBS node
    /// </summary>
    Task<bool> AddMemberAsync(Guid nodeId, AddOBSMemberDto dto);
    
    /// <summary>
    /// Remove member from OBS node
    /// </summary>
    Task<bool> RemoveMemberAsync(Guid nodeId, Guid userId);
    
    /// <summary>
    /// Get node members
    /// </summary>
    Task<IEnumerable<OBSMemberDto>> GetMembersAsync(Guid nodeId);
    
    /// <summary>
    /// Update capacity
    /// </summary>
    Task<bool> UpdateCapacityAsync(Guid nodeId, decimal totalFTE, decimal availableFTE);
    
    /// <summary>
    /// Get capacity report
    /// </summary>
    Task<OBSCapacityDto> GetCapacityAsync(Guid nodeId);
    
    /// <summary>
    /// Get resource allocations
    /// </summary>
    Task<IEnumerable<OBSResourceAllocationDto>> GetResourceAllocationsAsync(Guid nodeId);
    
    /// <summary>
    /// Assign node to project
    /// </summary>
    Task<bool> AssignToProjectAsync(Guid nodeId, Guid projectId);
    
    /// <summary>
    /// Copy OBS structure
    /// </summary>
    Task<OBSNodeDto?> CopyStructureAsync(Guid sourceNodeId, Guid? targetParentId, Guid? targetProjectId);
    
    /// <summary>
    /// Export OBS structure
    /// </summary>
    Task<byte[]> ExportAsync(Guid? projectId = null, string format = "Excel");
    
    /// <summary>
    /// Import OBS structure
    /// </summary>
    Task<int> ImportAsync(byte[] data, Guid? projectId = null, bool merge = false);
    
    /// <summary>
    /// Validate node code uniqueness
    /// </summary>
    Task<bool> IsCodeUniqueAsync(string code, Guid? projectId = null, Guid? excludeId = null);
}