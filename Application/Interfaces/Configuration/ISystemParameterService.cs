using Core.DTOs.Configuration.SystemParameters;

namespace Application.Interfaces.Configuration;

/// <summary>
/// Service for managing system parameters
/// </summary>
public interface ISystemParameterService
{
    /// <summary>
    /// Get all parameters
    /// </summary>
    Task<IEnumerable<SystemParameterDto>> GetAllAsync();
    
    /// <summary>
    /// Get parameter by ID
    /// </summary>
    Task<SystemParameterDto?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Get parameter by category and key
    /// </summary>
    Task<SystemParameterDto?> GetByKeyAsync(string category, string key);
    
    /// <summary>
    /// Get parameter value
    /// </summary>
    Task<string?> GetValueAsync(string category, string key);
    
    /// <summary>
    /// Get typed parameter value
    /// </summary>
    Task<T?> GetValueAsync<T>(string category, string key);
    
    /// <summary>
    /// Get parameters by category
    /// </summary>
    Task<IEnumerable<SystemParameterDto>> GetByCategoryAsync(string category);
    
    /// <summary>
    /// Get all categories
    /// </summary>
    Task<IEnumerable<SystemParameterCategoryDto>> GetCategoriesAsync();
    
    /// <summary>
    /// Search parameters
    /// </summary>
    Task<IEnumerable<SystemParameterDto>> SearchAsync(SystemParameterFilterDto filter);
    
    /// <summary>
    /// Create parameter
    /// </summary>
    Task<SystemParameterDto> CreateAsync(CreateSystemParameterDto dto);
    
    /// <summary>
    /// Update parameter
    /// </summary>
    Task<SystemParameterDto?> UpdateAsync(Guid id, UpdateSystemParameterDto dto);
    
    /// <summary>
    /// Update parameter value
    /// </summary>
    Task<bool> UpdateValueAsync(string category, string key, string value);
    
    /// <summary>
    /// Delete parameter (only non-system parameters)
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
    
    /// <summary>
    /// Validate parameter value
    /// </summary>
    Task<bool> ValidateValueAsync(string category, string key, string value);
    
    /// <summary>
    /// Export parameters
    /// </summary>
    Task<byte[]> ExportAsync(string? category = null);
    
    /// <summary>
    /// Import parameters
    /// </summary>
    Task<int> ImportAsync(byte[] data, bool overwriteExisting = false);
    
    /// <summary>
    /// Get public parameters (accessible without authentication)
    /// </summary>
    Task<IEnumerable<SystemParameterDto>> GetPublicParametersAsync();
    
    /// <summary>
    /// Reset parameter to default value
    /// </summary>
    Task<bool> ResetToDefaultAsync(string category, string key);
    
    /// <summary>
    /// Get parameter history
    /// </summary>
    Task<IEnumerable<ParameterHistoryDto>> GetHistoryAsync(Guid parameterId);
}
