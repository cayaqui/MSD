namespace Core.Enums;

/// <summary>
/// Tipos de actividades del cronograma
/// </summary>
public enum ActivityType
{
    /// <summary>
    /// Actividad dependiente de tareas
    /// </summary>
    TaskDependent,
    
    /// <summary>
    /// Actividad dependiente de recursos
    /// </summary>
    ResourceDependent,
    
    /// <summary>
    /// Fin a inicio
    /// </summary>
    FinishToStart,
    
    /// <summary>
    /// Inicio a inicio
    /// </summary>
    StartToStart,
    
    /// <summary>
    /// Fin a fin
    /// </summary>
    FinishToFinish,
    
    /// <summary>
    /// Hito de inicio
    /// </summary>
    StartMilestone,
    
    /// <summary>
    /// Hito de fin
    /// </summary>
    FinishMilestone
}