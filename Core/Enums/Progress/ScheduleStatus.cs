namespace Core.Enums.Progress
{
    /// <summary>
    /// Schedule Status
    /// </summary>
    public enum ScheduleStatus
    {
        Draft = 1,
        UnderReview = 2,
        InReview = 2, // Alias for UnderReview
        Approved = 3,
        Baselined = 4,
        Active = 4, // Alias for Baselined
        InProgress = 5,
        Completed = 6,
        Superseded = 7
    }

}
