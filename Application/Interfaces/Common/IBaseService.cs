namespace Application.Interfaces.Common
{
    public interface IBaseService<TDto, TCreateDto, TUpdateDto>
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<TDto> CreateAsync(TCreateDto createDto, string? createdBy = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PagedResult<TDto>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 20, string? orderBy = null, bool descending = false, CancellationToken cancellationToken = default);
        Task<TDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TDto?> UpdateAsync(Guid id, TUpdateDto updateDto, string? updatedBy = null, CancellationToken cancellationToken = default);
    }
}