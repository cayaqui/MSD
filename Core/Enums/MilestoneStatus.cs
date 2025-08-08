namespace Core.Enums;

/// <summary>
/// Estados de los hitos del proyecto
/// </summary>
public enum MilestoneStatus
{
    /// <summary>
    /// Pendiente
    /// </summary>
    Pending,
    
    /// <summary>
    /// Completado
    /// </summary>
    Completed,
    
    /// <summary>
    /// Aprobado
    /// </summary>
    Approved,
    
    /// <summary>
    /// Retrasado
    /// </summary>
    Delayed,
    
    /// <summary>
    /// En riesgo
    /// </summary>
    AtRisk
}