namespace Core.Enums.Progress
{
    /// <summary>
    /// Critical Path status
    /// </summary>
    public enum CriticalPathStatus
    {
        Critical = 1,
        NearCritical = 2,     // Float <= 5 days
        NonCritical = 3
    }

}
