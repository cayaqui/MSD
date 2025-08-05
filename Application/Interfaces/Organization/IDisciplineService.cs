using Application.Interfaces.Common;
using Core.DTOs.Organization.Discipline;
using Domain.Interfaces;

namespace Application.Interfaces.Organization
{
    /// <summary>
    /// Service interface for Discipline management
    /// </summary>
    public interface IDisciplineService : IBaseService<DisciplineDto, CreateDisciplineDto, UpdateDisciplineDto>
    {
        /// <summary>
        /// Gets a discipline by its code
        /// </summary>
        Task<DisciplineDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all engineering disciplines
        /// </summary>
        Task<IEnumerable<DisciplineDto>> GetEngineeringDisciplinesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all management disciplines
        /// </summary>
        Task<IEnumerable<DisciplineDto>> GetManagementDisciplinesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates discipline basic information
        /// </summary>
        Task<DisciplineDto?> UpdateBasicInfoAsync(Guid id, string name, string? description, int order, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates discipline visual settings
        /// </summary>
        Task<DisciplineDto?> UpdateVisualAsync(Guid id, string colorHex, string? icon, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates discipline category
        /// </summary>
        Task<DisciplineDto?> UpdateCategoryAsync(Guid id, bool isEngineering, bool isManagement, string? updatedBy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets disciplines ordered by their order field
        /// </summary>
        Task<IEnumerable<DisciplineDto>> GetOrderedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if discipline code is unique
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the unit of work instance
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}