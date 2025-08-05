using Application.Interfaces.Auth;
using Application.Services.Base;
using Core.DTOs.Auth.Permissions;
using Domain.Entities.Auth.Permissions;

namespace Application.Services.Auth;

public class PermissionService : BaseService<Permission, PermissionDto, CreatePermissionDto, UpdatePermissionDto>, IPermissionService
{
    public PermissionService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PermissionService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    public async Task<PermissionDto?> GetByCodeAsync(string code)
    {
        var permission = await _unitOfWork.Repository<Permission>()
            .GetAsync(p => p.Code == code && !p.IsDeleted);
        return permission != null ? _mapper.Map<PermissionDto>(permission) : null;
    }

    public async Task<IEnumerable<PermissionDto>> GetByModuleAsync(string module)
    {
        var permissions = await _unitOfWork.Repository<Permission>()
            .GetAllAsync(p => p.Module == module && !p.IsDeleted && p.IsActive);
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<PermissionDto>> GetByResourceAsync(string module, string resource)
    {
        var permissions = await _unitOfWork.Repository<Permission>()
            .GetAllAsync(p => p.Module == module && p.Resource == resource && !p.IsDeleted && p.IsActive);
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<PermissionMatrixDto> GetMatrixAsync()
    {
        var permissions = await _unitOfWork.Repository<Permission>()
            .GetAllAsync(p => !p.IsDeleted && p.IsActive);

        var matrix = new PermissionMatrixDto
        {
            Modules = permissions.Select(p => p.Module).Distinct().OrderBy(m => m).ToList(),
            Resources = permissions.Select(p => p.Resource).Distinct().OrderBy(r => r).ToList(),
            Actions = permissions.Select(p => p.Action).Distinct().OrderBy(a => a).ToList(),
            Permissions = _mapper.Map<List<PermissionDto>>(permissions)
        };

        return matrix;
    }

    public async Task<IEnumerable<PermissionDto>> SearchAsync(PermissionFilterDto filter)
    {
        var query = _unitOfWork.Repository<Permission>()
            .QueryNoTracking();

        query = query.Where(p => !p.IsDeleted);

        if (!string.IsNullOrEmpty(filter.Module))
            query = query.Where(p => p.Module == filter.Module);

        if (!string.IsNullOrEmpty(filter.Resource))
            query = query.Where(p => p.Resource == filter.Resource);

        if (!string.IsNullOrEmpty(filter.Action))
            query = query.Where(p => p.Action == filter.Action);

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Code.ToLower().Contains(searchTerm) ||
                p.DisplayName.ToLower().Contains(searchTerm) ||
                p.Description != null && p.Description.ToLower().Contains(searchTerm));
        }

        if (filter.IsActive.HasValue)
            query = query.Where(p => p.IsActive == filter.IsActive.Value);

        var permissions = await query
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Resource)
            .ThenBy(p => p.DisplayOrder)
            .ToListAsync();

        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var permission = await _unitOfWork.Repository<Permission>().GetByIdAsync(id);
        if (permission == null)
            return false;

        permission.Activate();
        _unitOfWork.Repository<Permission>().Update(permission);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Permission {PermissionCode} activated", permission.Code);
        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var permission = await _unitOfWork.Repository<Permission>().GetByIdAsync(id);
        if (permission == null)
            return false;

        permission.Deactivate();
        _unitOfWork.Repository<Permission>().Update(permission);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Permission {PermissionCode} deactivated", permission.Code);
        return true;
    }

    public async Task<int> BulkAssignToRolesAsync(BulkPermissionAssignmentDto dto)
    {
        //TODO: Implement bulk assignment of permissions to roles
        // This would require Role and RolePermission entities
        // Implementation would depend on the relationship structure
        throw new NotImplementedException("BulkAssignToRolesAsync requires Role entity implementation");
    }


    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        var query = _unitOfWork.Repository<Permission>()
            .QueryNoTracking();

        query = query.Where(p => p.Code == code && !p.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return !await query.AnyAsync();
    }

    public async Task<int> InitializeDefaultPermissionsAsync()
    {
        var existingCodes = (await _unitOfWork.Repository<Permission>()
            .GetAllAsync(p => !p.IsDeleted))
            .Select(p => p.Code)
            .ToHashSet();

        var defaultPermissions = GetDefaultPermissions()
            .Where(p => !existingCodes.Contains(p.Code))
            .ToList();

        if (defaultPermissions.Any())
        {
            await _unitOfWork.Repository<Permission>().AddRangeAsync(defaultPermissions);
            await _unitOfWork.SaveChangesAsync();
        }

        _logger.LogInformation("Initialized {Count} default permissions", defaultPermissions.Count);
        return defaultPermissions.Count;
    }

    public async Task<byte[]> ExportAsync(PermissionFilterDto? filter = null)
    {
        var permissions = filter != null 
            ? await SearchAsync(filter) 
            : await GetAllAsync();

        var json = JsonSerializer.Serialize(permissions, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    public async Task<int> ImportAsync(byte[] data, bool overwriteExisting = false)
    {
        var json = System.Text.Encoding.UTF8.GetString(data);
        var importedPermissions = JsonSerializer.Deserialize<List<CreatePermissionDto>>(json);

        if (importedPermissions == null || !importedPermissions.Any())
            return 0;

        var count = 0;
        foreach (var dto in importedPermissions)
        {
            var existing = await GetByCodeAsync(dto.Code);
            
            if (existing == null)
            {
                await CreateAsync(dto);
                count++;
            }
            else if (overwriteExisting)
            {
                var updateDto = _mapper.Map<UpdatePermissionDto>(dto);
                await UpdateAsync(existing.Id, updateDto);
                count++;
            }
        }

        _logger.LogInformation("Imported {Count} permissions", count);
        return count;
    }

    public async Task<PermissionValidationResult> ValidateStructureAsync()
    {
        var result = new PermissionValidationResult { IsValid = true };
        var permissions = await _unitOfWork.Repository<Permission>()
            .GetAllAsync(p => !p.IsDeleted);

        // Check for duplicates
        var duplicates = permissions
            .GroupBy(p => p.Code)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var dup in duplicates)
        {
            result.IsValid = false;
            result.DuplicatePermissions[dup.Key] = dup.Select(p => p.Id.ToString()).ToList();
            result.Errors.Add($"Duplicate permission code: {dup.Key}");
        }

        // Check for required permissions
        var requiredPermissions = GetRequiredPermissions();
        var existingCodes = permissions.Select(p => p.Code).ToHashSet();

        foreach (var (module, resourcePermissions) in requiredPermissions)
        {
            foreach (var (resource, actions) in resourcePermissions)
            {
                foreach (var action in actions)
                {
                    var code = Permission.CreateCode(resource, action);
                    if (!existingCodes.Contains(code))
                    {
                        if (!result.MissingPermissions.ContainsKey(module))
                            result.MissingPermissions[module] = new List<string>();
                        
                        result.MissingPermissions[module].Add(code);
                        result.Warnings.Add($"Missing permission: {code} in module {module}");
                    }
                }
            }
        }

        return result;
    }

    protected override async Task ValidateEntityAsync(Permission entity, bool isNew)
    {
        if (string.IsNullOrWhiteSpace(entity.Code))
            throw new InvalidOperationException("Permission code is required");

        if (string.IsNullOrWhiteSpace(entity.Module))
            throw new InvalidOperationException("Permission module is required");

        if (string.IsNullOrWhiteSpace(entity.Resource))
            throw new InvalidOperationException("Permission resource is required");

        if (string.IsNullOrWhiteSpace(entity.Action))
            throw new InvalidOperationException("Permission action is required");

        if (isNew)
        {
            var isUnique = await IsCodeUniqueAsync(entity.Code);
            if (!isUnique)
                throw new InvalidOperationException($"Permission code '{entity.Code}' already exists");
        }
    }

    private List<Permission> GetDefaultPermissions()
    {
        var permissions = new List<Permission>();
        var modules = new[] { "Projects", "Cost", "Schedule", "Quality", "Risk", "Documents", "Reports", "Configuration" };
        var commonActions = new[] { "View", "Create", "Edit", "Delete" };
        var approvalActions = new[] { "View", "Create", "Edit", "Delete", "Approve", "Reject" };

        // Project permissions
        foreach (var action in commonActions)
        {
            permissions.Add(new Permission(
                Permission.CreateCode("Project", action),
                "Projects",
                "Project",
                action,
                $"{action} Projects"
            ));
        }

        // Cost permissions
        foreach (var resource in new[] { "Budget", "Commitment", "Invoice" })
        {
            var actions = resource == "Budget" ? approvalActions : commonActions;
            foreach (var action in actions)
            {
                permissions.Add(new Permission(
                    Permission.CreateCode(resource, action),
                    "Cost",
                    resource,
                    action,
                    $"{action} {resource}"
                ));
            }
        }

        // Add more default permissions as needed...

        return permissions;
    }

    private Dictionary<string, Dictionary<string, List<string>>> GetRequiredPermissions()
    {
        return new Dictionary<string, Dictionary<string, List<string>>>
        {
            ["Projects"] = new Dictionary<string, List<string>>
            {
                ["Project"] = new List<string> { "View", "Create", "Edit", "Delete" }
            },
            ["Cost"] = new Dictionary<string, List<string>>
            {
                ["Budget"] = new List<string> { "View", "Create", "Edit", "Delete", "Approve" },
                ["Commitment"] = new List<string> { "View", "Create", "Edit", "Delete" }
            },
            ["Configuration"] = new Dictionary<string, List<string>>
            {
                ["User"] = new List<string> { "View", "Create", "Edit", "Delete" },
                ["Role"] = new List<string> { "View", "Create", "Edit", "Delete" }
            }
        };
    }
}