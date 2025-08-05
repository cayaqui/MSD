using Application.Interfaces.Common;
using Core.DTOs.Organization.OBSNode;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Organizational Breakdown Structure (OBS) management
    /// </summary>
    public interface IOBSNodeService : IBaseService<OBSNodeDto, CreateOBSNodeDto, UpdateOBSNodeDto>
    {
        /// <summary>
        /// Gets OBS nodes by project
        /// </summary>
        Task<IEnumerable<OBSNodeDto>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets OBS nodes by parent
        /// </summary>
        Task<IEnumerable<OBSNodeDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets OBS hierarchy tree
        /// </summary>
        Task<OBSNodeTreeDto> GetHierarchyTreeAsync(Guid? projectId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets node parent
        /// </summary>
        Task<OBSNodeDto?> SetParentAsync(Guid id, Guid? parentId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns node to project
        /// </summary>
        Task<OBSNodeDto?> AssignToProjectAsync(Guid id, Guid projectId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets node manager
        /// </summary>
        Task<OBSNodeDto?> SetManagerAsync(Guid id, Guid managerId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates node capacity
        /// </summary>
        Task<OBSNodeDto?> UpdateCapacityAsync(Guid id, decimal totalFTE, decimal availableFTE, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds member to OBS node
        /// </summary>
        Task<OBSNodeDto?> AddMemberAsync(Guid id, AddOBSNodeMemberDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes member from OBS node
        /// </summary>
        Task<OBSNodeDto?> RemoveMemberAsync(Guid id, Guid userId, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets node member count
        /// </summary>
        Task<int> GetMemberCountAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets node utilization rate
        /// </summary>
        Task<decimal> GetUtilizationRateAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Moves node to different parent
        /// </summary>
        Task<OBSNodeDto?> MoveNodeAsync(Guid id, MoveOBSNodeDto dto, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all descendants of a node
        /// </summary>
        Task<IEnumerable<OBSNodeDto>> GetDescendantsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates if move operation is valid (no circular references)
        /// </summary>
        Task<bool> CanMoveNodeAsync(Guid nodeId, Guid newParentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}