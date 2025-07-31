using Application.Interfaces.Auth;

namespace Application.Services.Auth;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UserService> logger,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<UserDto>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var (users, totalCount) = await _unitOfWork.Repository<User>()
            .GetPagedAsync(
                pageNumber,
                pageSize,
                filter: u => !u.IsDeleted,
                orderBy: q => q.OrderBy(u => u.Name),
                includeProperties: "ProjectTeamMembers.Project");

        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return new PagedResult<UserDto>
        {
            Items = userDtos.ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<UserDto>> SearchAsync(UserFilterDto filter)
    {
        var query = _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, filter);

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply ordering
        query = query.OrderBy(u => u.Name);

        // Apply pagination
        var users = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return new PagedResult<UserDto>
        {
            Items = userDtos.ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail && !u.IsDeleted);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetByEntraIdAsync(string entraId)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .FirstOrDefaultAsync(u => u.EntraId == entraId && !u.IsDeleted);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<IEnumerable<ProjectTeamMemberDto>> GetUserProjectsAsync(Guid userId)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
            return Enumerable.Empty<ProjectTeamMemberDto>();

        return _mapper.Map<IEnumerable<ProjectTeamMemberDto>>(
            user.ProjectTeamMembers.Where(ptm => ptm.IsActive));
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLowerInvariant() || u.EntraId == dto.EntraId);

        if (existingUser != null)
        {
            if (existingUser.IsDeleted)
            {
                // Restore deleted user
                existingUser.Restore();
                existingUser.UpdateAzureProfile(new AzureAdUser
                {
                    Id = dto.EntraId,
                    Mail = dto.Email,
                    DisplayName = dto.Name,
                    GivenName = dto.GivenName,
                    Surname = dto.Surname,
                    MobilePhone = dto.PhoneNumber,
                    JobTitle = dto.JobTitle,
                    PreferredLanguage = dto.PreferredLanguage
                });

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Restored deleted user {Email}", dto.Email);

                return _mapper.Map<UserDto>(existingUser);
            }

            throw new BadRequestException($"User with email {dto.Email} already exists");
        }

        // Create new user
        var user = new User(dto.EntraId, dto.Email, dto.Name);

        if (!string.IsNullOrEmpty(dto.GivenName) || !string.IsNullOrEmpty(dto.Surname))
        {
            user.UpdatePersonalInfo(
                dto.GivenName,
                dto.Surname,
                dto.PhoneNumber,
                dto.JobTitle,
                dto.PreferredLanguage);
        }

        user.CreatedBy = _currentUserService.UserId;

        await _unitOfWork.Repository<User>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Created new user {Email} with ID {UserId}", user.Email, user.Id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
                .ThenInclude(ptm => ptm.Project)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (user == null)
            return null;

        // Update personal info if provided
        if (!string.IsNullOrEmpty(dto.Name) ||
            !string.IsNullOrEmpty(dto.GivenName) ||
            !string.IsNullOrEmpty(dto.Surname))
        {
            user.UpdatePersonalInfo(
                dto.GivenName ?? user.GivenName,
                dto.Surname ?? user.Surname,
                dto.PhoneNumber ?? user.PhoneNumber,
                dto.JobTitle ?? user.JobTitle,
                dto.PreferredLanguage ?? user.PreferredLanguage);
        }

        if (!string.IsNullOrEmpty(dto.CompanyId))
        {
            user.CompanyId = dto.CompanyId;
        }

        user.UpdatedBy = _currentUserService.UserId;
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Updated user {UserId}", id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> ActivateAsync(Guid id)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        user.Activate();
        user.UpdatedBy = _currentUserService.UserId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Activated user {UserId}", id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> DeactivateAsync(Guid id)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        // Check if user can be deactivated
        if (await HasActiveProjectsAsync(id))
        {
            throw new BadRequestException("Cannot deactivate user with active project assignments");
        }

        user.Deactivate();
        user.UpdatedBy = _currentUserService.UserId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Deactivated user {UserId}", id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteAsync(Guid id, string? deletedBy = null)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new NotFoundException(nameof(User), id);

        // Check if user can be deleted
        if (!await CanUserBeDeletedAsync(id))
        {
            throw new BadRequestException("User cannot be deleted due to active assignments or dependencies");
        }

        user.Delete(deletedBy ?? _currentUserService.UserId);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Deleted user {UserId} by {DeletedBy}", id, deletedBy ?? _currentUserService.UserId);
    }

    public async Task<UserDto?> SyncWithAzureADAsync(Guid id)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (user == null)
            return null;

        // In a real implementation, this would call Microsoft Graph API
        // For now, we'll just return the existing user
        _logger.LogWarning("Azure AD sync not implemented. Returning existing user data.");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateOrUpdateFromAzureADAsync(AzureAdUser azureUser)
    {
        var existingUser = await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.EntraId == azureUser.Id ||
                                     u.Email == azureUser.Mail.ToLowerInvariant());

        if (existingUser != null)
        {
            // Update existing user
            existingUser.UpdateAzureProfile(azureUser);

            if (existingUser.IsDeleted)
            {
                existingUser.Restore();
            }

            if (!existingUser.IsActive && azureUser.AccountEnabled)
            {
                existingUser.Activate();
            }
            else if (existingUser.IsActive && !azureUser.AccountEnabled)
            {
                existingUser.Deactivate();
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated user {Email} from Azure AD", existingUser.Email);

            return _mapper.Map<UserDto>(existingUser);
        }

        // Create new user
        var newUser = new User(
            azureUser.Id,
            azureUser.Mail,
            azureUser.DisplayName);

        newUser.UpdatePersonalInfo(
            azureUser.GivenName,
            azureUser.Surname,
            azureUser.MobilePhone,
            azureUser.JobTitle,
            azureUser.PreferredLanguage);

        if (!azureUser.AccountEnabled)
        {
            newUser.Deactivate();
        }

        await _unitOfWork.Repository<User>().AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Created user {Email} from Azure AD", newUser.Email);

        return _mapper.Map<UserDto>(newUser);
    }

    public async Task<int> BulkActivateAsync(List<Guid> userIds)
    {
        var users = await _unitOfWork.Repository<User>()
            .Query()
            .Where(u => userIds.Contains(u.Id) && !u.IsActive)
            .ToListAsync();

        foreach (var user in users)
        {
            user.Activate();
            user.UpdatedBy = _currentUserService.UserId;
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bulk activated {Count} users", users.Count);

        return users.Count;
    }

    public async Task<int> BulkDeactivateAsync(List<Guid> userIds)
    {
        var users = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .Where(u => userIds.Contains(u.Id) && u.IsActive)
            .ToListAsync();

        var usersToDeactivate = new List<User>();

        foreach (var user in users)
        {
            // Skip users with active projects
            if (!user.HasActiveProjects())
            {
                user.Deactivate();
                user.UpdatedBy = _currentUserService.UserId;
                usersToDeactivate.Add(user);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Bulk deactivated {Count} users", usersToDeactivate.Count);

        return usersToDeactivate.Count;
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var query = _unitOfWork.Repository<User>()
            .Query()
            .Where(u => u.Email == normalizedEmail && !u.IsDeleted);

        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> CanUserBeDeletedAsync(Guid id)
    {
        var user = await _unitOfWork.Repository<User>()
            .Query()
            .Include(u => u.ProjectTeamMembers)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return false;

        // Cannot delete if user has active project assignments
        if (user.HasActiveProjects())
            return false;

        // Add other business rules as needed
        // For example: check if user has created any important records

        return true;
    }

    private IQueryable<User> ApplyFilters(IQueryable<User> query, UserFilterDto filter)
    {
        // Base filter - exclude deleted unless requested
        if (!filter.IncludeDeleted.GetValueOrDefault())
        {
            query = query.Where(u => !u.IsDeleted);
        }

        // Search term
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(u =>
                u.Name.ToLower().Contains(searchTerm) ||
                u.Email.ToLower().Contains(searchTerm) ||
                (u.GivenName != null && u.GivenName.ToLower().Contains(searchTerm)) ||
                (u.Surname != null && u.Surname.ToLower().Contains(searchTerm)) ||
                (u.JobTitle != null && u.JobTitle.ToLower().Contains(searchTerm)));
        }

        // Active filter
        if (filter.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == filter.IsActive.Value);
        }

        // Company filter
        if (!string.IsNullOrEmpty(filter.CompanyId))
        {
            query = query.Where(u => u.CompanyId == filter.CompanyId);
        }

        // Project filter
        if (filter.ProjectId.HasValue)
        {
            query = query.Where(u => u.ProjectTeamMembers.Any(ptm =>
                ptm.ProjectId == filter.ProjectId.Value && ptm.IsActive));
        }

        // Role filter (in any project)
        if (!string.IsNullOrEmpty(filter.Role))
        {
            query = query.Where(u => u.ProjectTeamMembers.Any(ptm =>
                ptm.Role == filter.Role && ptm.IsActive));
        }

        // Last login filter
        if (filter.LastLoginFrom.HasValue)
        {
            query = query.Where(u => u.LastLoginAt >= filter.LastLoginFrom.Value);
        }

        if (filter.LastLoginTo.HasValue)
        {
            query = query.Where(u => u.LastLoginAt <= filter.LastLoginTo.Value);
        }

        return query;
    }

    private async Task<bool> HasActiveProjectsAsync(Guid userId)
    {
        return await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .AnyAsync(ptm => ptm.UserId == userId &&
                            ptm.IsActive &&
                            (!ptm.EndDate.HasValue || ptm.EndDate.Value > DateTime.UtcNow));
    }
}