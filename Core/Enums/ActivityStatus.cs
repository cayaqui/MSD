namespace Core.Enums;

/// <summary>
/// Estados de las actividades del cronograma
/// </summary>
public enum ActivityStatus
{
    /// <summary>
    /// No iniciada
    /// </summary>
    NotStarted,
    
    /// <summary>
    /// En progreso
    /// </summary>
    InProgress,
    
    /// <summary>
    /// Completada
    /// </summary>
    Completed,
    
    /// <summary>
    /// Suspendida
    /// </summary>
    Suspended,
    
    /// <summary>
    /// Cancelada
    /// </summary>
    Cancelled
}