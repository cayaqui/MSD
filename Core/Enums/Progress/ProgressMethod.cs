namespace Core.Enums.Progress
{
    /// <summary>
    /// Progress calculation method
    /// </summary>
    public enum ProgressMethod
    {
        Manual = 1,              // Manual entry
        WeightedActivities = 2,  // Based on activity weights
        Milestones = 3,          // Based on milestone completion
        UnitsProduced = 4,       // Based on units completed
        DurationBased = 5        // Based on elapsed time
    }

}
