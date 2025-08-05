using Core.DTOs.Common;
using Core.DTOs.Organization.RAM;

namespace Web.Services.Interfaces.Organization;

/// <summary>
/// Service interface for RAM (Responsibility Assignment Matrix) API operations
/// </summary>
public interface IRAMApiService
{
    // Query Operations
    
    /// <summary>
    /// Get all RAM assignments with pagination
    /// </summary>
    Task<PagedResult<RAMDto>?> GetRAMAssignmentsAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get RAM assignment by ID
    /// </summary>
    Task<RAMDto?> GetRAMByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get RAM assignments by project
    /// </summary>
    Task<List<RAMDto>?> GetRAMByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get project RAM matrix
    /// </summary>
    Task<RAMMatrixDto?> GetProjectRAMMatrixAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user RAM assignments
    /// </summary>
    Task<List<UserRAMAssignmentDto>?> GetUserRAMAssignmentsAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get RAM assignments by OBS node
    /// </summary>
    Task<List<RAMDto>?> GetRAMByOBSNodeAsync(Guid obsNodeId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Export RAM matrix
    /// </summary>
    Task<byte[]?> ExportRAMMatrixAsync(Guid projectId, CancellationToken cancellationToken = default);

    // Command Operations
    
    /// <summary>
    /// Create a new RAM assignment
    /// </summary>
    Task<RAMDto?> CreateRAMAsync(CreateRAMDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing RAM assignment
    /// </summary>
    Task<RAMDto?> UpdateRAMAsync(Guid id, UpdateRAMDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Bulk assign RAM
    /// </summary>
    Task<List<RAMDto>?> BulkAssignRAMAsync(List<CreateRAMDto> dtos, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Import RAM matrix
    /// </summary>
    Task<RAMMatrixDto?> ImportRAMMatrixAsync(Guid projectId, byte[] fileData, string fileName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add notes to RAM assignment
    /// </summary>
    Task<RAMDto?> AddRAMNotesAsync(Guid id, AddRAMNotesDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete RAM assignment (soft delete)
    /// </summary>
    Task<bool> DeleteRAMAsync(Guid id, CancellationToken cancellationToken = default);
}