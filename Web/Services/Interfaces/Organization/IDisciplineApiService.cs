using Core.DTOs.Common;
using Core.DTOs.Organization.Discipline;
using Core.Enums.Projects;

namespace Web.Services.Interfaces.Organization;

/// <summary>
/// Service interface for discipline API operations
/// </summary>
public interface IDisciplineApiService
{
    /// <summary>
    /// Get all disciplines with pagination
    /// </summary>
    Task<PagedResult<DisciplineDto>?> GetDisciplinesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool isAscending = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get discipline by ID
    /// </summary>
    Task<DisciplineDto?> GetDisciplineByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active disciplines
    /// </summary>
    Task<List<DisciplineDto>?> GetActiveDisciplinesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get disciplines by type
    /// </summary>
    Task<List<DisciplineDto>?> GetDisciplinesByTypeAsync(DisciplineType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new discipline
    /// </summary>
    Task<DisciplineDto?> CreateDisciplineAsync(CreateDisciplineDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing discipline
    /// </summary>
    Task<DisciplineDto?> UpdateDisciplineAsync(Guid id, UpdateDisciplineDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a discipline (soft delete)
    /// </summary>
    Task<bool> DeleteDisciplineAsync(Guid id, CancellationToken cancellationToken = default);
}