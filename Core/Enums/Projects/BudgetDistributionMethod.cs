namespace Core.Enums.Projects;

/// <summary>
/// Methods for distributing budget across time periods
/// </summary>
public enum BudgetDistributionMethod
{
    /// <summary>
    /// Distribute evenly across all periods
    /// </summary>
    EvenDistribution = 1,
    
    /// <summary>
    /// Front-load distribution (more budget in earlier periods)
    /// </summary>
    FrontLoaded = 2,
    
    /// <summary>
    /// Back-load distribution (more budget in later periods)
    /// </summary>
    BackLoaded = 3,
    
    /// <summary>
    /// Bell curve distribution (peak in middle periods)
    /// </summary>
    BellCurve = 4,
    
    /// <summary>
    /// Custom distribution based on provided values
    /// </summary>
    Custom = 5
}