using Application.Interfaces.Auth;
using Application.Interfaces.UIUX;
using Domain.Entities.Projects;

namespace Application.Services.UIUX;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return Enumerable.Empty<NotificationDto>();

        var notifications = await _unitOfWork.Repository<Notification>()
            .Query()
            .Include(n => n.Project)
            .Include(n => n.Company)
            .Where(n => n.UserId == user.Id && !n.IsDeleted && !n.IsExpired)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50) // Limit to recent 50 notifications
            .ToListAsync();

        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<PagedResult<NotificationDto>> GetPagedNotificationsAsync(NotificationFilterDto filter)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return new PagedResult<NotificationDto> { Items = new List<NotificationDto>() };

        var query = _unitOfWork.Repository<Notification>()
            .Query()
            .Include(n => n.Project)
            .Include(n => n.Company)
            .Where(n => n.UserId == user.Id && !n.IsDeleted);

        // Apply filters
        query = ApplyFilters(query, filter);

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply ordering
        query = query.OrderByDescending(n => n.CreatedAt);

        // Apply pagination
        var notifications = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

        return new PagedResult<NotificationDto>
        {
            Items = notificationDtos.ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
        };
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return 0;

        return await _unitOfWork.Repository<Notification>()
            .Query()
            .CountAsync(n =>
                n.UserId == user.Id &&
                n.Status == NotificationStatus.Unread &&
                !n.IsDeleted &&
                !n.IsExpired);
    }

    public async Task<NotificationSummaryDto> GetSummaryAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return new NotificationSummaryDto();

        var notifications = await _unitOfWork.Repository<Notification>()
            .Query()
            .Where(n => n.UserId == user.Id && !n.IsDeleted && !n.IsExpired)
            .ToListAsync();

        var summary = new NotificationSummaryDto
        {
            TotalCount = notifications.Count,
            UnreadCount = notifications.Count(n => n.Status == NotificationStatus.Unread),
            ImportantCount = notifications.Count(n => n.IsImportant),
            UnreadImportantCount = notifications.Count(n => n.IsImportant && n.Status == NotificationStatus.Unread),
            LastNotificationAt = notifications.OrderByDescending(n => n.CreatedAt).FirstOrDefault()?.CreatedAt
        };

        // Count by type
        summary.CountByType = notifications
            .GroupBy(n => n.Type)
            .ToDictionary(g => g.Key, g => g.Count());

        return summary;
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            throw new UnauthorizedException();

        var notification = await _unitOfWork.Repository<Notification>()
            .Query()
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == user.Id);

        if (notification == null)
            throw new NotFoundException(nameof(Notification), notificationId);

        notification.MarkAsRead();
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Notification {NotificationId} marked as read", notificationId);
    }

    public async Task MarkAllAsReadAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            throw new UnauthorizedException();

        var unreadNotifications = await _unitOfWork.Repository<Notification>()
            .Query()
            .Where(n =>
                n.UserId == user.Id &&
                n.Status == NotificationStatus.Unread &&
                !n.IsDeleted)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Marked {Count} notifications as read for user {UserId}",
            unreadNotifications.Count, user.Id);
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
    {
        // If no user specified, use current user
        if (dto.UserId == null && dto.UserIds == null)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser != null)
                dto.UserId = currentUser.Id;
        }

        var notifications = new List<Notification>();

        // Create notification for single user
        if (dto.UserId.HasValue)
        {
            var notification = CreateNotificationEntity(dto, dto.UserId.Value);
            await SetNotificationContext(notification, dto);
            notifications.Add(notification);
        }
        // Create notifications for multiple users
        else if (dto.UserIds != null && dto.UserIds.Any())
        {
            foreach (var userId in dto.UserIds.Distinct())
            {
                var notification = CreateNotificationEntity(dto, userId);
                await SetNotificationContext(notification, dto);
                notifications.Add(notification);
            }
        }

        if (!notifications.Any())
            throw new BadRequestException("No target users specified for notification");

        await _unitOfWork.Repository<Notification>().AddRangeAsync(notifications);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Created {Count} notifications", notifications.Count);

        // Return the first notification as DTO
        return _mapper.Map<NotificationDto>(notifications.First());
    }

    public async Task CreateProjectNotificationAsync(
        Guid projectId,
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        ProjectNotificationCategory category = ProjectNotificationCategory.General)
    {
        // Get all active team members for the project
        var teamMembers = await _unitOfWork.Repository<ProjectTeamMember>()
            .Query()
            .Where(ptm => ptm.ProjectId == projectId && ptm.IsActive)
            .Select(ptm => ptm.UserId)
            .ToListAsync();

        if (!teamMembers.Any())
        {
            _logger.LogWarning("No team members found for project {ProjectId}", projectId);
            return;
        }

        var dto = new CreateNotificationDto
        {
            Title = title,
            Message = message,
            Type = type,
            ProjectId = projectId,
            UserIds = teamMembers,
            Metadata = new Dictionary<string, object>
            {
                { "category", category.ToString() }
            }
        };

        await CreateAsync(dto);
    }

    public async Task CreateBulkNotificationAsync(
        string title,
        string message,
        NotificationType type,
        Expression<Func<User, bool>> userFilter)
    {
        var users = await _unitOfWork.Repository<User>()
            .Query()
            .Where(userFilter)
            .Where(u => u.IsActive && !u.IsDeleted)
            .Select(u => u.Id)
            .ToListAsync();

        if (!users.Any())
        {
            _logger.LogWarning("No users matched the filter for bulk notification");
            return;
        }

        var dto = new CreateNotificationDto
        {
            Title = title,
            Message = message,
            Type = type,
            UserIds = users
        };

        await CreateAsync(dto);

        _logger.LogInformation("Sent bulk notification to {Count} users", users.Count);
    }

    public async Task ProcessBatchActionAsync(BatchNotificationDto dto)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            throw new UnauthorizedException();

        var notifications = await _unitOfWork.Repository<Notification>()
            .Query()
            .Where(n => dto.NotificationIds.Contains(n.Id) && n.UserId == user.Id)
            .ToListAsync();

        if (!notifications.Any())
            throw new NotFoundException("No notifications found for the specified IDs");

        foreach (var notification in notifications)
        {
            switch (dto.Action)
            {
                case NotificationBatchAction.MarkAsRead:
                    notification.MarkAsRead();
                    break;
                case NotificationBatchAction.MarkAsUnread:
                    notification.MarkAsUnread();
                    break;
                case NotificationBatchAction.Archive:
                    notification.Archive();
                    break;
                case NotificationBatchAction.Delete:
                    notification.IsDeleted = true;
                    notification.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Processed batch action {Action} for {Count} notifications",
            dto.Action, notifications.Count);
    }

    public async Task DeleteNotificationAsync(Guid notificationId)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            throw new UnauthorizedException();

        var notification = await _unitOfWork.Repository<Notification>()
            .Query()
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == user.Id);

        if (notification == null)
            throw new NotFoundException(nameof(Notification), notificationId);

        notification.IsDeleted = true;
        notification.DeletedAt = DateTime.UtcNow;
        notification.DeletedBy = _currentUserService.UserId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Notification {NotificationId} deleted", notificationId);
    }

    public async Task<NotificationPreferencesDto> GetUserPreferencesAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            throw new UnauthorizedException();

        // This would typically come from a separate UserPreferences entity
        // For now, return default preferences
        return new NotificationPreferencesDto
        {
            UserId = user.Id,
            EmailEnabled = true,
            EmailOnImportant = true,
            EmailOnProjectUpdates = true,
            EmailOnTeamChanges = true,
            InAppEnabled = true,
            ShowDesktopNotifications = false,
            PlaySound = true,
            EnabledTypes = Enum.GetValues<NotificationType>().ToList(),
            MutedProjects = new List<Guid>()
        };
    }

    public async Task UpdateUserPreferencesAsync(NotificationPreferencesDto preferences)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            throw new UnauthorizedException();

        // This would typically update a separate UserPreferences entity
        // Implementation would depend on your preference storage strategy

        _logger.LogInformation("Updated notification preferences for user {UserId}", user.Id);
    }

    // Helper methods
    private async Task<User?> GetCurrentUserAsync()
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            return null;

        return await _unitOfWork.Repository<User>()
            .Query()
            .FirstOrDefaultAsync(u => u.EntraId == _currentUserService.UserId && !u.IsDeleted);
    }

    private Notification CreateNotificationEntity(CreateNotificationDto dto, Guid userId)
    {
        var notification = new Notification(
            dto.Title,
            dto.Message,
            userId,
            dto.Type,
            dto.Priority
        );

        if (dto.IsImportant)
            notification.SetImportant(true);

        if (!string.IsNullOrEmpty(dto.ActionUrl))
            notification.SetActionUrl(dto.ActionUrl);

        if (dto.ExpiresAt.HasValue)
            notification.SetExpiration(dto.ExpiresAt.Value);

        if (dto.Metadata != null)
            notification.SetMetadata(JsonSerializer.Serialize(dto.Metadata));

        return notification;
    }

    private async Task SetNotificationContext(Notification notification, CreateNotificationDto dto)
    {
        if (dto.ProjectId.HasValue)
        {
            var project = await _unitOfWork.Repository<Project>()
                .Query()
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId.Value);

            if (project != null)
                notification.SetProject(project);
        }

        if (dto.CompanyId.HasValue)
        {
            var company = await _unitOfWork.Repository<Company>()
                .Query()
                .FirstOrDefaultAsync(c => c.Id == dto.CompanyId.Value);

            if (company != null)
                notification.SetCompany(company);
        }
    }

    private IQueryable<Notification> ApplyFilters(IQueryable<Notification> query, NotificationFilterDto filter)
    {
        // Search term
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(n =>
                n.Title.ToLower().Contains(searchTerm) ||
                n.Message.ToLower().Contains(searchTerm));
        }

        // Status filter
        if (filter.Status.HasValue)
        {
            query = query.Where(n => n.Status == filter.Status.Value);
        }

        // Type filter
        if (filter.Type.HasValue)
        {
            query = query.Where(n => n.Type == filter.Type.Value);
        }

        // Priority filter
        if (filter.Priority.HasValue)
        {
            query = query.Where(n => n.Priority == filter.Priority.Value);
        }

        // Important filter
        if (filter.IsImportant.HasValue)
        {
            query = query.Where(n => n.IsImportant == filter.IsImportant.Value);
        }

        // Date range filters
        if (filter.CreatedFrom.HasValue)
        {
            query = query.Where(n => n.CreatedAt >= filter.CreatedFrom.Value);
        }

        if (filter.CreatedTo.HasValue)
        {
            query = query.Where(n => n.CreatedAt <= filter.CreatedTo.Value);
        }

        // Context filters
        if (filter.ProjectId.HasValue)
        {
            query = query.Where(n => n.ProjectId == filter.ProjectId.Value);
        }

        if (filter.CompanyId.HasValue)
        {
            query = query.Where(n => n.CompanyId == filter.CompanyId.Value);
        }

        // Exclude expired notifications
        query = query.Where(n => !n.ExpiresAt.HasValue || n.ExpiresAt.Value > DateTime.UtcNow);

        return query;
    }
}