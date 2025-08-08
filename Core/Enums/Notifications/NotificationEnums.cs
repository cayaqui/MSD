namespace Core.Enums.Notifications;

public enum NotificationType
{
    System,
    ProjectUpdate,
    TaskAssignment,
    TaskCompletion,
    MilestoneReached,
    RiskIdentified,
    RiskStatusChange,
    DocumentUploaded,
    DocumentApproved,
    DocumentRejected,
    BudgetAlert,
    CostOverrun,
    ScheduleDelay,
    ContractUpdate,
    ChangeOrderCreated,
    ApprovalRequired,
    ApprovalGranted,
    ApprovalRejected,
    CommentAdded,
    MentionedInComment,
    TeamMemberAdded,
    TeamMemberRemoved,
    PermissionGranted,
    PermissionRevoked,
    ReportGenerated,
    DataImported,
    DataExported,
    SystemMaintenance,
    SecurityAlert
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent,
    Critical
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Delivered,
    Read,
    Failed,
    Expired,
    Archived
}

public enum NotificationChannel
{
    InApp,
    Email,
    Push,
    SMS,
    Teams,
    Slack
}

public enum NotificationFrequency
{
    Immediate,
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Never
}

public enum NotificationCategory
{
    Project,
    Cost,
    Schedule,
    Risk,
    Document,
    Contract,
    Team,
    System,
    Security,
    Report,
    Approval,
    Communication
}