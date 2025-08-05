using Application.Interfaces.Common;
using Application.Interfaces.Configuration;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Configuration.SystemParameters;
using Domain.Entities.Configuration.Core;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Configuration;

/// <summary>
/// Service implementation for managing system parameters
/// </summary>
public class SystemParameterService : ISystemParameterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SystemParameterService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SystemParameterDto>> GetAllAsync()
    {
        var parameters = await _unitOfWork.Repository<SystemParameter>()
            .GetAllAsync(orderBy: q => q.OrderBy(p => p.Category).ThenBy(p => p.Key));
        
        return _mapper.Map<IEnumerable<SystemParameterDto>>(parameters);
    }

    public async Task<SystemParameterDto?> GetByIdAsync(Guid id)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Id == id);
        
        return parameter != null ? _mapper.Map<SystemParameterDto>(parameter) : null;
    }

    public async Task<SystemParameterDto?> GetByKeyAsync(string category, string key)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Category == category && p.Key == key);
        
        return parameter != null ? _mapper.Map<SystemParameterDto>(parameter) : null;
    }

    public async Task<string?> GetValueAsync(string category, string key)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Category == category && p.Key == key);
        
        return parameter?.Value;
    }

    public async Task<T?> GetValueAsync<T>(string category, string key)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Category == category && p.Key == key);
        
        if (parameter == null)
            return default;

        return parameter.GetValueAs<T>();
    }

    public async Task<IEnumerable<SystemParameterDto>> GetByCategoryAsync(string category)
    {
        var parameters = await _unitOfWork.Repository<SystemParameter>()
            .GetAllAsync(
                filter: p => p.Category == category,
                orderBy: q => q.OrderBy(p => p.Key));
        
        return _mapper.Map<IEnumerable<SystemParameterDto>>(parameters);
    }

    public async Task<IEnumerable<SystemParameterCategoryDto>> GetCategoriesAsync()
    {
        var parameters = await _unitOfWork.Repository<SystemParameter>()
            .Query()
            .GroupBy(p => p.Category)
            .Select(g => new SystemParameterCategoryDto
            {
                Category = g.Key,
                Description = g.Key, // Could be enhanced with a category description table
                ParameterCount = g.Count(),
                // Additional counts could be added if needed
                // SystemParameterCount = g.Count(p => p.IsSystem),
                // PublicParameterCount = g.Count(p => p.IsPublic)
            })
            .OrderBy(c => c.Category)
            .ToListAsync();

        return parameters;
    }

    public async Task<IEnumerable<SystemParameterDto>> SearchAsync(SystemParameterFilterDto filter)
    {
        var query = _unitOfWork.Repository<SystemParameter>().Query();

        if (!string.IsNullOrWhiteSpace(filter.Category))
            query = query.Where(p => p.Category == filter.Category);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Key.ToLower().Contains(searchTerm) ||
                p.DisplayName!.ToLower().Contains(searchTerm) ||
                p.Description!.ToLower().Contains(searchTerm));
        }

        if (filter.IsSystem.HasValue)
            query = query.Where(p => p.IsSystem == filter.IsSystem.Value);

        if (filter.IsPublic.HasValue)
            query = query.Where(p => p.IsPublic == filter.IsPublic.Value);

        if (!string.IsNullOrWhiteSpace(filter.DataType))
            query = query.Where(p => p.DataType == filter.DataType);

        var parameters = await query
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Key)
            .ToListAsync();

        return _mapper.Map<IEnumerable<SystemParameterDto>>(parameters);
    }

    public async Task<SystemParameterDto> CreateAsync(CreateSystemParameterDto dto)
    {
        // Check if parameter already exists
        var existing = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Category == dto.Category && p.Key == dto.Key);

        if (existing != null)
            throw new InvalidOperationException($"Parameter with key '{dto.Key}' already exists in category '{dto.Category}'");

        var parameter = new SystemParameter(
            dto.Category,
            dto.Key,
            dto.Value,
            dto.DataType,
            false); // IsSystem is always false for user-created parameters

        parameter.UpdateMetadata(
            dto.DisplayName,
            dto.Description,
            dto.IsRequired,
            dto.IsPublic);

        if (dto.DefaultValue != null || dto.MinValue.HasValue || dto.MaxValue.HasValue || dto.AllowedValues?.Any() == true)
        {
            parameter.SetValidationRules(
                dto.ValidationRule,
                dto.MinValue,
                dto.MaxValue,
                dto.AllowedValues);
        }

        await _unitOfWork.Repository<SystemParameter>().AddAsync(parameter);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SystemParameterDto>(parameter);
    }

    public async Task<SystemParameterDto?> UpdateAsync(Guid id, UpdateSystemParameterDto dto)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Id == id);

        if (parameter == null)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Value))
            parameter.UpdateValue(dto.Value);

        parameter.UpdateMetadata(
            dto.DisplayName,
            dto.Description,
            dto.IsRequired,
            dto.IsPublic);

        if (dto.MinValue.HasValue || dto.MaxValue.HasValue || dto.AllowedValues?.Any() == true)
        {
            parameter.SetValidationRules(
                dto.ValidationRule,
                dto.MinValue,
                dto.MaxValue,
                dto.AllowedValues);
        }

        _unitOfWork.Repository<SystemParameter>().Update(parameter);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SystemParameterDto>(parameter);
    }

    public async Task<bool> UpdateValueAsync(string category, string key, string value)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Category == category && p.Key == key);

        if (parameter == null)
            return false;

        parameter.UpdateValue(value);
        
        _unitOfWork.Repository<SystemParameter>().Update(parameter);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Id == id);

        if (parameter == null)
            return false;

        if (parameter.IsSystem)
            throw new InvalidOperationException("System parameters cannot be deleted");

        _unitOfWork.Repository<SystemParameter>().Remove(parameter);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ValidateValueAsync(string category, string key, string value)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Category == category && p.Key == key);

        if (parameter == null)
            return false;

        try
        {
            // Create a temporary parameter to test validation
            var temp = new SystemParameter(category, key, value, parameter.DataType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<byte[]> ExportAsync(string? category = null)
    {
        // This would export parameters to Excel/JSON format
        // Implementation would depend on the export library being used
        throw new NotImplementedException("Export functionality to be implemented");
    }

    public async Task<int> ImportAsync(byte[] data, bool overwriteExisting = false)
    {
        // This would import parameters from Excel/JSON format
        // Implementation would depend on the import library being used
        throw new NotImplementedException("Import functionality to be implemented");
    }

    public async Task<IEnumerable<SystemParameterDto>> GetPublicParametersAsync()
    {
        var parameters = await _unitOfWork.Repository<SystemParameter>()
            .GetAllAsync(
                filter: p => p.IsPublic,
                orderBy: q => q.OrderBy(p => p.Category).ThenBy(p => p.Key));

        return _mapper.Map<IEnumerable<SystemParameterDto>>(parameters);
    }

    public async Task<bool> ResetToDefaultAsync(string category, string key)
    {
        var parameter = await _unitOfWork.Repository<SystemParameter>()
            .GetAsync(filter: p => p.Category == category && p.Key == key);

        if (parameter == null || string.IsNullOrEmpty(parameter.DefaultValue))
            return false;

        parameter.UpdateValue(parameter.DefaultValue);
        
        _unitOfWork.Repository<SystemParameter>().Update(parameter);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ParameterHistoryDto>> GetHistoryAsync(Guid parameterId)
    {
        // This would require an audit log implementation
        // For now, returning empty list
        return new List<ParameterHistoryDto>();
    }
}