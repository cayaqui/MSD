namespace Core.Enums.Setup;
public enum ProjectStatus
{
    Planning = 1,
    Active = 2,
    OnHold = 3,
    Completed = 4,
    Cancelled = 5,
    Delayed = 6,
    Closed = 7
}

public static class ProjectStatusExtensions
{
    public static string GetDisplayName(this ProjectStatus status)
    {
        return status switch
        {
            ProjectStatus.Planning => "En Planificación",
            ProjectStatus.Active => "En Progreso",
            ProjectStatus.OnHold => "En Espera",
            ProjectStatus.Completed => "Completado",
            ProjectStatus.Cancelled => "Cancelado",
            ProjectStatus.Delayed => "Retrasado",
            ProjectStatus.Closed => "Cerrado",
            _ => status.ToString()
        };
    }

    public static string GetCssClass(this ProjectStatus status)
    {
        return status switch
        {
            ProjectStatus.Planning => "primary",
            ProjectStatus.Active => "info",
            ProjectStatus.OnHold => "warning",
            ProjectStatus.Completed => "success",
            ProjectStatus.Cancelled => "danger",
            ProjectStatus.Delayed => "secondary",
            ProjectStatus.Closed => "dark",
            _ => "light"
        };
    }

    public static bool IsActive(this ProjectStatus status)
    {
        return status is ProjectStatus.Planning or ProjectStatus.Active or ProjectStatus.Delayed;
    }

    public static bool IsFinished(this ProjectStatus status)
    {
        return status is ProjectStatus.Completed or ProjectStatus.Cancelled;
    }
}