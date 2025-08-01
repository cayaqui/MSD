namespace Core.Enums.Setup;

public enum PhaseStatus
{
    NotStarted = 1,
    InProgress = 2,
    OnHold = 3,
    Completed = 4,
    Cancelled = 5,
    Delayed = 6
}

public static class PhaseStatusExtensions
{
    public static string GetDisplayName(this PhaseStatus status)
    {
        return status switch
        {
            PhaseStatus.NotStarted => "No Iniciado",
            PhaseStatus.InProgress => "En Progreso",
            PhaseStatus.Completed => "Completado",
            PhaseStatus.Delayed => "Retrasado",
            PhaseStatus.Cancelled => "Cancelado",
            _ => status.ToString()
        };
    }

    public static string GetCssClass(this PhaseStatus status)
    {
        return status switch
        {
            PhaseStatus.NotStarted => "secondary",
            PhaseStatus.InProgress => "info",
            PhaseStatus.Completed => "success",
            PhaseStatus.Delayed => "warning",
            PhaseStatus.Cancelled => "danger",
            _ => "light"
        };
    }

    public static bool IsActive(this PhaseStatus status)
    {
        return status is PhaseStatus.InProgress or PhaseStatus.Delayed;
    }

    public static bool CanStart(this PhaseStatus status)
    {
        return status is PhaseStatus.NotStarted;
    }

    public static bool IsFinished(this PhaseStatus status)
    {
        return status is PhaseStatus.Completed or PhaseStatus.Cancelled;
    }
}