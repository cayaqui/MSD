using Application.Interfaces.Notifications;
using Application.Interfaces.Common;
using Application.Interfaces.Auth;
using Application.Interfaces.Projects;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Notifications;
using Core.Enums.Notifications;
using Core.Results;
using Domain.Entities.Notifications;
using Domain.Entities.Auth.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    private readonly IProjectTeamMemberService _projectTeamMemberService;

    public NotificationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<NotificationService> logger,
        ICurrentUserService currentUserService,
        IEmailService emailService,
        IProjectTeamMemberService projectTeamMemberService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _emailService = emailService;
        _projectTeamMemberService = projectTeamMemberService;
    }

    #region Notification Operations

    public async Task<PagedResult<NotificationDto>> GetUserNotificationsAsync(
        Guid userId,
        NotificationQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Notification>()
            .Query()
            .Where(n => n.RecipientId == userId);

        // Apply filters
        if (parameters.Type.HasValue)
            query = query.Where(n => n.Type == parameters.Type.Value);

        if (parameters.Priority.HasValue)
            query = query.Where(n => n.Priority == parameters.Priority.Value);

        if (!string.IsNullOrWhiteSpace(parameters.Category))
            query = query.Where(n => n.Category == parameters.Category);

        if (parameters.IsRead.HasValue)
        {
            if (parameters.IsRead.Value)
                query = query.Where(n => n.Status == NotificationStatus.Read);
            else
                query = query.Where(n => n.Status != NotificationStatus.Read && n.Status != NotificationStatus.Archived);
        }

        if (parameters.IsArchived.HasValue)
        {
            if (parameters.IsArchived.Value)
                query = query.Where(n => n.Status == NotificationStatus.Archived);
            else
                query = query.Where(n => n.Status != NotificationStatus.Archived);
        }

        if (parameters.StartDate.HasValue)
            query = query.Where(n => n.CreatedAt >= parameters.StartDate.Value);

        if (parameters.EndDate.HasValue)
            query = query.Where(n => n.CreatedAt <= parameters.EndDate.Value);

        if (parameters.RelatedEntityId.HasValue)
            query = query.Where(n => n.RelatedEntityId == parameters.RelatedEntityId.Value);

        // Apply search
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(n =>
                n.Title.ToLower().Contains(searchTerm) ||
                n.Message.ToLower().Contains(searchTerm));
        }

        // Apply sorting
        query = parameters.SortBy?.ToLower() switch
        {
            "date" => parameters.IsAscending
                ? query.OrderBy(n => n.CreatedAt)
                : query.OrderByDescending(n => n.CreatedAt),
            "priority" => parameters.IsAscending
                ? query.OrderBy(n => n.Priority)
                : query.OrderByDescending(n => n.Priority),
            "type" => parameters.IsAscending
                ? query.OrderBy(n => n.Type)
                : query.OrderByDescending(n => n.Type),
            _ => query.OrderByDescending(n => n.CreatedAt)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(n => n.RecipientUser)
            .Include(n => n.SenderUser)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResult<NotificationDto>
        {
            Items = dtos,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
        };
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Repository<Notification>()
            .GetAsync(
                filter: n => n.Id == notificationId,
                includeProperties: "RecipientUser,SenderUser",
                cancellationToken: cancellationToken);

        return notification != null ? MapToDto(notification) : null;
    }

    public async Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<Notification>()
            .CountAsync(
                n => n.RecipientId == userId &&
                     n.Status != NotificationStatus.Read &&
                     n.Status != NotificationStatus.Archived &&
                     n.Status != NotificationStatus.Expired,
                cancellationToken);
    }

    public async Task<Core.Results.Result<Guid>> CreateNotificationAsync(
        CreateNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new Notification(
                dto.Type,
                dto.Priority,
                dto.Title,
                dto.Message,
                dto.RecipientId,
                dto.SenderId,
                dto.Category);

            notification.CreatedBy = _currentUserService.UserId ?? "System";

            if (dto.RelatedEntityId.HasValue)
            {
                notification.SetRelatedEntity(
                    dto.RelatedEntityId.Value,
                    dto.RelatedEntityType ?? "",
                    dto.RelatedEntityName ?? "",
                    dto.ActionUrl);
            }

            if (dto.Metadata != null && dto.Metadata.Any())
            {
                notification.SetMetadata(JsonSerializer.Serialize(dto.Metadata));
            }

            if (dto.Actions != null && dto.Actions.Any())
            {
                notification.SetActions(JsonSerializer.Serialize(dto.Actions));
            }

            await _unitOfWork.Repository<Notification>().AddAsync(notification);

            // Check user preferences and send via appropriate channels
            await SendNotificationViaChannels(notification, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(notification.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create notification");
            return Result<Guid>.Failure($"Failed to create notification: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result<int>> CreateBulkNotificationsAsync(
        BulkCreateNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notifications = new List<Notification>();

            foreach (var recipientId in dto.RecipientIds)
            {
                var notification = new Notification(
                    dto.Type,
                    dto.Priority,
                    dto.Title,
                    dto.Message,
                    recipientId,
                    dto.SenderId,
                    dto.Category);

                notification.CreatedBy = _currentUserService.UserId ?? "System";

                if (dto.RelatedEntityId.HasValue)
                {
                    notification.SetRelatedEntity(
                        dto.RelatedEntityId.Value,
                        dto.RelatedEntityType ?? "",
                        dto.RelatedEntityName ?? "",
                        dto.ActionUrl);
                }

                notifications.Add(notification);
            }

            await _unitOfWork.Repository<Notification>().AddRangeAsync(notifications);

            // Send notifications via channels
            foreach (var notification in notifications)
            {
                await SendNotificationViaChannels(notification, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(notifications.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create bulk notifications");
            return Result<int>.Failure($"Failed to create bulk notifications: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _unitOfWork.Repository<Notification>()
                .GetAsync(
                    filter: n => n.Id == notificationId && n.RecipientId == userId,
                    cancellationToken: cancellationToken);

            if (notification == null)
                return Result.Failure("Notification not found");

            notification.MarkAsRead();
            notification.UpdatedBy = _currentUserService.UserId;

            _unitOfWork.Repository<Notification>().Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark notification as read");
            return Result.Failure($"Failed to mark notification as read: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> MarkMultipleAsReadAsync(
        List<Guid> notificationIds,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notifications = await _unitOfWork.Repository<Notification>()
                .GetAllAsync(
                    filter: n => notificationIds.Contains(n.Id) && n.RecipientId == userId,
                    cancellationToken: cancellationToken);

            foreach (var notification in notifications)
            {
                notification.MarkAsRead();
                notification.UpdatedBy = _currentUserService.UserId;
            }

            _unitOfWork.Repository<Notification>().UpdateRange(notifications);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark multiple notifications as read");
            return Result.Failure($"Failed to mark notifications as read: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notifications = await _unitOfWork.Repository<Notification>()
                .GetAllAsync(
                    filter: n => n.RecipientId == userId &&
                               n.Status != NotificationStatus.Read &&
                               n.Status != NotificationStatus.Archived,
                    cancellationToken: cancellationToken);

            foreach (var notification in notifications)
            {
                notification.MarkAsRead();
                notification.UpdatedBy = _currentUserService.UserId;
            }

            _unitOfWork.Repository<Notification>().UpdateRange(notifications);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark all notifications as read");
            return Result.Failure($"Failed to mark all notifications as read: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> ArchiveNotificationAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _unitOfWork.Repository<Notification>()
                .GetAsync(
                    filter: n => n.Id == notificationId && n.RecipientId == userId,
                    cancellationToken: cancellationToken);

            if (notification == null)
                return Result.Failure("Notification not found");

            notification.MarkAsArchived();
            notification.UpdatedBy = _currentUserService.UserId;

            _unitOfWork.Repository<Notification>().Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to archive notification");
            return Result.Failure($"Failed to archive notification: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> DeleteNotificationAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _unitOfWork.Repository<Notification>()
                .GetAsync(
                    filter: n => n.Id == notificationId && n.RecipientId == userId,
                    cancellationToken: cancellationToken);

            if (notification == null)
                return Result.Failure("Notification not found");

            _unitOfWork.Repository<Notification>().Delete(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete notification");
            return Result.Failure($"Failed to delete notification: {ex.Message}");
        }
    }

    #endregion

    #region Template Operations

    public async Task<List<NotificationTemplateDto>> GetTemplatesAsync(
        bool? activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<NotificationTemplate>()
            .Query()
            .Where(t => !t.IsDeleted);

        if (activeOnly.HasValue && activeOnly.Value)
            query = query.Where(t => t.IsActive);

        var templates = await query.ToListAsync(cancellationToken);

        return templates.Select(MapTemplateToDto).ToList();
    }

    public async Task<NotificationTemplateDto?> GetTemplateByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.Repository<NotificationTemplate>()
            .GetAsync(
                filter: t => t.Code == code && !t.IsDeleted,
                cancellationToken: cancellationToken);

        return template != null ? MapTemplateToDto(template) : null;
    }

    public async Task<Core.Results.Result<Guid>> CreateTemplateAsync(
        CreateNotificationTemplateDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if template with same code exists
            var existingTemplate = await _unitOfWork.Repository<NotificationTemplate>()
                .AnyAsync(t => t.Code == dto.Code && !t.IsDeleted, cancellationToken);

            if (existingTemplate)
                return Result<Guid>.Failure("Template with this code already exists");

            var template = new NotificationTemplate(
                dto.Code,
                dto.Name,
                dto.Description,
                dto.Type,
                dto.Category,
                dto.SubjectTemplate,
                dto.BodyTemplate);

            template.CreatedBy = userId;

            template.ConfigureEmail(
                dto.SendEmail,
                dto.EmailSubjectTemplate,
                dto.EmailBodyTemplate,
                dto.IsHtmlEmail);

            template.ConfigureInApp(
                dto.SendInApp,
                dto.DefaultPriority,
                dto.ExpirationDays);

            template.ConfigurePush(
                dto.SendPush,
                dto.PushTitleTemplate,
                dto.PushBodyTemplate);

            await _unitOfWork.Repository<NotificationTemplate>().AddAsync(template);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(template.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create notification template");
            return Result<Guid>.Failure($"Failed to create template: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> UpdateTemplateAsync(
        Guid templateId,
        UpdateNotificationTemplateDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _unitOfWork.Repository<NotificationTemplate>()
                .GetAsync(
                    filter: t => t.Id == templateId && !t.IsDeleted,
                    cancellationToken: cancellationToken);

            if (template == null)
                return Result.Failure("Template not found");

            if (!string.IsNullOrWhiteSpace(dto.Name) ||
                !string.IsNullOrWhiteSpace(dto.Description) ||
                !string.IsNullOrWhiteSpace(dto.SubjectTemplate) ||
                !string.IsNullOrWhiteSpace(dto.BodyTemplate))
            {
                template.UpdateDetails(
                    dto.Name ?? template.Name,
                    dto.Description ?? template.Description,
                    dto.SubjectTemplate ?? template.SubjectTemplate,
                    dto.BodyTemplate ?? template.BodyTemplate);
            }

            if (dto.SendEmail.HasValue ||
                !string.IsNullOrWhiteSpace(dto.EmailSubjectTemplate) ||
                !string.IsNullOrWhiteSpace(dto.EmailBodyTemplate) ||
                dto.IsHtmlEmail.HasValue)
            {
                template.ConfigureEmail(
                    dto.SendEmail ?? template.SendEmail,
                    dto.EmailSubjectTemplate ?? template.EmailSubjectTemplate,
                    dto.EmailBodyTemplate ?? template.EmailBodyTemplate,
                    dto.IsHtmlEmail ?? template.IsHtmlEmail);
            }

            if (dto.SendInApp.HasValue ||
                dto.DefaultPriority.HasValue ||
                dto.ExpirationDays.HasValue)
            {
                template.ConfigureInApp(
                    dto.SendInApp ?? template.SendInApp,
                    dto.DefaultPriority ?? template.DefaultPriority,
                    dto.ExpirationDays ?? template.ExpirationDays);
            }

            if (dto.SendPush.HasValue ||
                !string.IsNullOrWhiteSpace(dto.PushTitleTemplate) ||
                !string.IsNullOrWhiteSpace(dto.PushBodyTemplate))
            {
                template.ConfigurePush(
                    dto.SendPush ?? template.SendPush,
                    dto.PushTitleTemplate ?? template.PushTitleTemplate,
                    dto.PushBodyTemplate ?? template.PushBodyTemplate);
            }

            if (dto.IsActive.HasValue)
            {
                if (dto.IsActive.Value)
                    template.Activate();
                else
                    template.Deactivate();
            }

            template.UpdatedBy = userId;

            _unitOfWork.Repository<NotificationTemplate>().Update(template);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update notification template");
            return Result.Failure($"Failed to update template: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> DeleteTemplateAsync(
        Guid templateId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _unitOfWork.Repository<NotificationTemplate>()
                .GetAsync(
                    filter: t => t.Id == templateId && !t.IsDeleted,
                    cancellationToken: cancellationToken);

            if (template == null)
                return Result.Failure("Template not found");

            template.IsDeleted = true;
            template.DeletedAt = DateTime.UtcNow;
            template.DeletedBy = userId;

            _unitOfWork.Repository<NotificationTemplate>().Update(template);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete notification template");
            return Result.Failure($"Failed to delete template: {ex.Message}");
        }
    }

    #endregion

    #region Preference Operations

    public async Task<NotificationPreferenceDto?> GetUserPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var preference = await _unitOfWork.Repository<NotificationPreference>()
            .GetAsync(
                filter: p => p.UserId == userId,
                includeProperties: "User,Subscriptions",
                cancellationToken: cancellationToken);

        if (preference == null)
        {
            // Create default preferences for user
            preference = new NotificationPreference(userId);
            preference.CreatedBy = _currentUserService.UserId ?? "System";

            await _unitOfWork.Repository<NotificationPreference>().AddAsync(preference);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return MapPreferenceToDto(preference);
    }

    public async Task<Core.Results.Result> UpdateUserPreferencesAsync(
        Guid userId,
        UpdateNotificationPreferenceDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var preference = await _unitOfWork.Repository<NotificationPreference>()
                .GetAsync(
                    filter: p => p.UserId == userId,
                    cancellationToken: cancellationToken);

            if (preference == null)
            {
                preference = new NotificationPreference(userId);
                preference.CreatedBy = _currentUserService.UserId ?? "System";
                await _unitOfWork.Repository<NotificationPreference>().AddAsync(preference);
            }

            preference.UpdateGlobalPreferences(
                dto.EnableEmailNotifications,
                dto.EnableInAppNotifications,
                dto.EnablePushNotifications);

            if (dto.EnableQuietHours.HasValue)
            {
                preference.ConfigureQuietHours(
                    dto.EnableQuietHours.Value,
                    dto.QuietHoursStart,
                    dto.QuietHoursEnd,
                    dto.TimeZone);
            }

            if (dto.CategoryPreferences != null)
            {
                preference.SetCategoryPreferences(JsonSerializer.Serialize(dto.CategoryPreferences));
            }

            if (dto.TypePreferences != null)
            {
                preference.SetTypePreferences(JsonSerializer.Serialize(dto.TypePreferences));
            }

            preference.UpdatedBy = _currentUserService.UserId;

            _unitOfWork.Repository<NotificationPreference>().Update(preference);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user preferences");
            return Result.Failure($"Failed to update preferences: {ex.Message}");
        }
    }

    public async Task<List<NotificationSubscriptionDto>> GetUserSubscriptionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var subscriptions = await _unitOfWork.Repository<NotificationSubscription>()
            .GetAllAsync(
                filter: s => s.UserId == userId && s.IsActive,
                cancellationToken: cancellationToken);

        return subscriptions.Select(MapSubscriptionToDto).ToList();
    }

    public async Task<Core.Results.Result<Guid>> SubscribeToEntityAsync(
        Guid userId,
        CreateNotificationSubscriptionDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if already subscribed
            var existingSubscription = await _unitOfWork.Repository<NotificationSubscription>()
                .GetAsync(
                    filter: s => s.UserId == userId &&
                               s.EntityId == dto.EntityId &&
                               s.SubscriptionType == dto.SubscriptionType,
                    cancellationToken: cancellationToken);

            if (existingSubscription != null)
            {
                if (!existingSubscription.IsActive)
                {
                    existingSubscription.Activate();
                    existingSubscription.UpdateSubscribedEvents(JsonSerializer.Serialize(dto.SubscribedEvents));
                    existingSubscription.UpdatedBy = _currentUserService.UserId;
                    
                    _unitOfWork.Repository<NotificationSubscription>().Update(existingSubscription);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    
                    return Result<Guid>.Success(existingSubscription.Id);
                }

                return Result<Guid>.Failure("Already subscribed to this entity");
            }

            // Get entity name
            var entityName = await GetEntityName(dto.SubscriptionType, dto.EntityId, cancellationToken);

            var subscription = new NotificationSubscription(
                userId,
                dto.SubscriptionType,
                dto.EntityId,
                entityName,
                JsonSerializer.Serialize(dto.SubscribedEvents));

            subscription.CreatedBy = _currentUserService.UserId ?? "System";

            await _unitOfWork.Repository<NotificationSubscription>().AddAsync(subscription);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(subscription.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to entity");
            return Result<Guid>.Failure($"Failed to subscribe: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> UnsubscribeFromEntityAsync(
        Guid subscriptionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var subscription = await _unitOfWork.Repository<NotificationSubscription>()
                .GetAsync(
                    filter: s => s.Id == subscriptionId && s.UserId == userId,
                    cancellationToken: cancellationToken);

            if (subscription == null)
                return Result.Failure("Subscription not found");

            subscription.Deactivate();
            subscription.UpdatedBy = _currentUserService.UserId;

            _unitOfWork.Repository<NotificationSubscription>().Update(subscription);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe from entity");
            return Result.Failure($"Failed to unsubscribe: {ex.Message}");
        }
    }

    #endregion

    #region Notification Sending

    public async Task<Core.Results.Result> SendNotificationFromTemplateAsync(
        string templateCode,
        Guid recipientId,
        Dictionary<string, string> placeholders,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _unitOfWork.Repository<NotificationTemplate>()
                .GetAsync(
                    filter: t => t.Code == templateCode && t.IsActive && !t.IsDeleted,
                    cancellationToken: cancellationToken);

            if (template == null)
                return Result.Failure("Notification template not found or inactive");

            // Replace placeholders in subject and body
            var subject = ReplacePlaceholders(template.SubjectTemplate, placeholders);
            var body = ReplacePlaceholders(template.BodyTemplate, placeholders);

            var notification = new Notification(
                template.Type,
                template.DefaultPriority,
                subject,
                body,
                recipientId,
                _currentUserService.UserId != null ? Guid.Parse(_currentUserService.UserId) : null,
                template.Category);

            notification.CreatedBy = _currentUserService.UserId ?? "System";

            if (relatedEntityId.HasValue)
            {
                var entityName = relatedEntityType != null
                    ? await GetEntityName(relatedEntityType, relatedEntityId.Value, cancellationToken)
                    : "";

                notification.SetRelatedEntity(
                    relatedEntityId.Value,
                    relatedEntityType ?? "",
                    entityName,
                    placeholders.ContainsKey("ActionUrl") ? placeholders["ActionUrl"] : null);
            }

            if (template.ExpirationDays.HasValue)
            {
                notification.SetExpirationDate(DateTime.UtcNow.AddDays(template.ExpirationDays.Value));
            }

            if (!string.IsNullOrWhiteSpace(template.DefaultActionsJson))
            {
                // Replace placeholders in actions
                var actions = JsonSerializer.Deserialize<List<NotificationTemplateActionDto>>(template.DefaultActionsJson);
                if (actions != null)
                {
                    foreach (var action in actions)
                    {
                        action.ActionUrlTemplate = ReplacePlaceholders(action.ActionUrlTemplate, placeholders);
                    }
                    notification.SetActions(JsonSerializer.Serialize(actions));
                }
            }

            await _unitOfWork.Repository<Notification>().AddAsync(notification);

            // Send via channels based on template configuration
            await SendNotificationViaChannelsWithTemplate(notification, template, placeholders, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification from template");
            return Result.Failure($"Failed to send notification: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> SendProjectNotificationAsync(
        Guid projectId,
        NotificationType type,
        string title,
        string message,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get project team members
            var teamMembers = await _projectTeamMemberService.GetProjectTeamMembersAsync(projectId, cancellationToken);

            var recipientIds = teamMembers
                .Where(tm => tm.IsActive && (!excludeUserId.HasValue || tm.UserId != excludeUserId.Value))
                .Select(tm => tm.UserId)
                .Distinct()
                .ToList();

            if (!recipientIds.Any())
                return Result.Success(); // No recipients

            var bulkDto = new BulkCreateNotificationDto
            {
                RecipientIds = recipientIds,
                Type = type,
                Priority = NotificationPriority.Normal,
                Title = title,
                Message = message,
                Category = "Project",
                SenderId = _currentUserService.UserId != null ? Guid.Parse(_currentUserService.UserId) : null,
                RelatedEntityId = projectId,
                RelatedEntityType = "Project"
            };

            var result = await CreateBulkNotificationsAsync(bulkDto, cancellationToken);

            return result.IsSuccess ? Result.Success() : Result.Failure(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send project notification");
            return Result.Failure($"Failed to send project notification: {ex.Message}");
        }
    }

    public async Task<Core.Results.Result> SendRoleNotificationAsync(
        string roleName,
        NotificationType type,
        string title,
        string message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get users with the specified role
            var users = await _unitOfWork.Repository<User>()
                .GetAllAsync(
                    filter: u => u.IsActive,
                    cancellationToken: cancellationToken);

            // Filter by role (would need role checking logic here)
            var recipientIds = users.Select(u => u.Id).ToList();

            if (!recipientIds.Any())
                return Result.Success(); // No recipients

            var bulkDto = new BulkCreateNotificationDto
            {
                RecipientIds = recipientIds,
                Type = type,
                Priority = NotificationPriority.Normal,
                Title = title,
                Message = message,
                Category = "System",
                SenderId = _currentUserService.UserId != null ? Guid.Parse(_currentUserService.UserId) : null
            };

            var result = await CreateBulkNotificationsAsync(bulkDto, cancellationToken);

            return result.IsSuccess ? Result.Success() : Result.Failure(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send role notification");
            return Result.Failure($"Failed to send role notification: {ex.Message}");
        }
    }

    #endregion

    #region Private Methods

    private NotificationDto MapToDto(Notification notification)
    {
        var dto = _mapper.Map<NotificationDto>(notification);

        if (!string.IsNullOrWhiteSpace(notification.MetadataJson))
        {
            try
            {
                dto.Metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(notification.MetadataJson) ?? new();
            }
            catch { }
        }

        if (!string.IsNullOrWhiteSpace(notification.ActionsJson))
        {
            try
            {
                dto.Actions = JsonSerializer.Deserialize<List<NotificationActionDto>>(notification.ActionsJson) ?? new();
            }
            catch { }
        }

        dto.IsRead = notification.Status == NotificationStatus.Read;
        dto.IsArchived = notification.Status == NotificationStatus.Archived;
        dto.RecipientName = notification.RecipientUser?.DisplayName ?? "";
        dto.RecipientEmail = notification.RecipientUser?.Email ?? "";
        dto.SenderName = notification.SenderUser?.DisplayName;

        return dto;
    }

    private NotificationTemplateDto MapTemplateToDto(NotificationTemplate template)
    {
        var dto = _mapper.Map<NotificationTemplateDto>(template);

        if (!string.IsNullOrWhiteSpace(template.PlaceholdersJson))
        {
            try
            {
                dto.AvailablePlaceholders = JsonSerializer.Deserialize<List<string>>(template.PlaceholdersJson) ?? new();
            }
            catch { }
        }

        if (!string.IsNullOrWhiteSpace(template.DefaultActionsJson))
        {
            try
            {
                dto.DefaultActions = JsonSerializer.Deserialize<List<NotificationTemplateActionDto>>(template.DefaultActionsJson) ?? new();
            }
            catch { }
        }

        return dto;
    }

    private NotificationPreferenceDto MapPreferenceToDto(NotificationPreference preference)
    {
        var dto = _mapper.Map<NotificationPreferenceDto>(preference);

        dto.UserName = preference.User?.DisplayName ?? "";
        dto.UserEmail = preference.User?.Email ?? "";

        if (!string.IsNullOrWhiteSpace(preference.CategoryPreferencesJson))
        {
            try
            {
                dto.CategoryPreferences = JsonSerializer.Deserialize<List<CategoryPreferenceDto>>(preference.CategoryPreferencesJson) ?? new();
            }
            catch { }
        }

        if (!string.IsNullOrWhiteSpace(preference.TypePreferencesJson))
        {
            try
            {
                dto.TypePreferences = JsonSerializer.Deserialize<List<TypePreferenceDto>>(preference.TypePreferencesJson) ?? new();
            }
            catch { }
        }

        return dto;
    }

    private NotificationSubscriptionDto MapSubscriptionToDto(NotificationSubscription subscription)
    {
        var dto = _mapper.Map<NotificationSubscriptionDto>(subscription);

        if (!string.IsNullOrWhiteSpace(subscription.SubscribedEventsJson))
        {
            try
            {
                dto.SubscribedEvents = JsonSerializer.Deserialize<List<string>>(subscription.SubscribedEventsJson) ?? new();
            }
            catch { }
        }

        return dto;
    }

    private async Task SendNotificationViaChannels(
        Notification notification,
        CancellationToken cancellationToken)
    {
        // Get user preferences
        var preference = await _unitOfWork.Repository<NotificationPreference>()
            .GetAsync(
                filter: p => p.UserId == notification.RecipientId,
                includeProperties: "User",
                cancellationToken: cancellationToken);

        if (preference == null)
        {
            // Create default preferences
            preference = new NotificationPreference(notification.RecipientId);
        }

        // Check if within quiet hours
        if (preference.IsWithinQuietHours(DateTime.UtcNow))
            return;

        // Send in-app notification (always created, delivery depends on preference)
        if (preference.EnableInAppNotifications)
        {
            notification.AddDelivery(NotificationChannel.InApp);
            notification.MarkAsSent();
            notification.MarkAsDelivered();
        }

        // Send email if enabled
        if (preference.EnableEmailNotifications && preference.User != null)
        {
            await SendEmailNotification(notification, preference.User.Email, cancellationToken);
        }
    }

    private async Task SendNotificationViaChannelsWithTemplate(
        Notification notification,
        NotificationTemplate template,
        Dictionary<string, string> placeholders,
        CancellationToken cancellationToken)
    {
        // Get user preferences
        var preference = await _unitOfWork.Repository<NotificationPreference>()
            .GetAsync(
                filter: p => p.UserId == notification.RecipientId,
                includeProperties: "User",
                cancellationToken: cancellationToken);

        if (preference == null)
        {
            preference = new NotificationPreference(notification.RecipientId);
        }

        // Check if within quiet hours
        if (preference.IsWithinQuietHours(DateTime.UtcNow))
            return;

        // Send in-app notification
        if (template.SendInApp && preference.EnableInAppNotifications)
        {
            notification.AddDelivery(NotificationChannel.InApp);
            notification.MarkAsSent();
            notification.MarkAsDelivered();
        }

        // Send email if enabled
        if (template.SendEmail && preference.EnableEmailNotifications && preference.User != null)
        {
            var emailSubject = ReplacePlaceholders(template.EmailSubjectTemplate ?? template.SubjectTemplate, placeholders);
            var emailBody = ReplacePlaceholders(template.EmailBodyTemplate ?? template.BodyTemplate, placeholders);

            await SendEmailNotification(
                notification,
                preference.User.Email,
                cancellationToken,
                emailSubject,
                emailBody,
                template.IsHtmlEmail);
        }
    }

    private async Task SendEmailNotification(
        Notification notification,
        string recipientEmail,
        CancellationToken cancellationToken,
        string? customSubject = null,
        string? customBody = null,
        bool isHtml = false)
    {
        try
        {
            var result = await _emailService.SendEmailAsync(
                recipientEmail,
                customSubject ?? notification.Title,
                customBody ?? notification.Message,
                isHtml,
                cancellationToken);

            var delivery = notification.Deliveries.FirstOrDefault(d => d.Channel == NotificationChannel.Email);
            if (delivery == null)
            {
                notification.AddDelivery(NotificationChannel.Email, recipientEmail);
                delivery = notification.Deliveries.First(d => d.Channel == NotificationChannel.Email);
            }

            delivery.SetRecipientAddress(recipientEmail);
            delivery.RecordAttempt(result.IsSuccess, result.IsSuccess ? null : result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email notification");
        }
    }

    private string ReplacePlaceholders(string template, Dictionary<string, string> placeholders)
    {
        var result = template;

        foreach (var placeholder in placeholders)
        {
            result = result.Replace($"{{{placeholder.Key}}}", placeholder.Value);
        }

        return result;
    }

    private async Task<string> GetEntityName(string entityType, Guid entityId, CancellationToken cancellationToken)
    {
        // This would need to be implemented based on your entity types
        // For now, returning the entity type and ID
        return await Task.FromResult($"{entityType} {entityId}");
    }

    #endregion
}