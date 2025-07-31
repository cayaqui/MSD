namespace Core.Enums.Projects;

/// <summary>
/// Tipos de elementos en la estructura de desglose de trabajo (WBS)
/// </summary>
public enum WBSElementType
{
    /// <summary>
    /// Nivel de proyecto
    /// </summary>
    Project = 1,

    /// <summary>
    /// Fase del proyecto
    /// </summary>
    Phase = 2,

    /// <summary>
    /// Entregable
    /// </summary>
    Deliverable = 3,

    /// <summary>
    /// Paquete de trabajo - nivel más bajo ejecutable
    /// </summary>
    WorkPackage = 4,

    /// <summary>
    /// Paquete de planificación - trabajo futuro no detallado
    /// </summary>
    PlanningPackage = 5,

    /// <summary>
    /// Elemento resumen - nodo intermedio
    /// </summary>
    Summary = 6,

    /// <summary>
    /// Hito del proyecto
    /// </summary>
    Milestone = 7
}

/// <summary>
/// Extension methods para WBSElementType
/// </summary>
public static class WBSElementTypeExtensions
{
    public static string GetDisplayName(this WBSElementType type)
    {
        return type switch
        {
            WBSElementType.Project => "Proyecto",
            WBSElementType.Phase => "Fase",
            WBSElementType.Deliverable => "Entregable",
            WBSElementType.WorkPackage => "Paquete de Trabajo",
            WBSElementType.PlanningPackage => "Paquete de Planificación",
            WBSElementType.Summary => "Resumen",
            WBSElementType.Milestone => "Hito",
            _ => type.ToString()
        };
    }

    public static string GetIcon(this WBSElementType type)
    {
        return type switch
        {
            WBSElementType.Project => "bi-diagram-3",
            WBSElementType.Phase => "bi-collection",
            WBSElementType.Deliverable => "bi-box",
            WBSElementType.WorkPackage => "bi-card-checklist",
            WBSElementType.PlanningPackage => "bi-calendar-week",
            WBSElementType.Summary => "bi-folder",
            WBSElementType.Milestone => "bi-flag",
            _ => "bi-question"
        };
    }

    public static bool CanHaveChildren(this WBSElementType type)
    {
        return type != WBSElementType.WorkPackage && type != WBSElementType.PlanningPackage;
    }

    public static bool IsExecutable(this WBSElementType type)
    {
        return type == WBSElementType.WorkPackage;
    }
}