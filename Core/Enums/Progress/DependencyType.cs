namespace Core.Enums.Progress
{
    /// <summary>
    /// Dependency Type for activities
    /// </summary>
    public enum DependencyType
    {
        FinishToStart = 1,    // FS - Most common
        StartToStart = 2,     // SS
        FinishToFinish = 3,   // FF
        StartToFinish = 4     // SF - Rare
    }

}
